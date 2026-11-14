import * as webllm from "https://cdn.jsdelivr.net/npm/@mlc-ai/web-llm@0.2.88/+esm";

const MODEL_ID = "Llama-3.2-1B-Instruct-q4f16_1-MLC";
let engine = null;

export async function isSupported() {
    if (!navigator.gpu) return false;
    try {
        return (await navigator.gpu.requestAdapter()) !== null;
    } catch {
        return false;
    }
}

export async function isCached() {
    try {
        return await webllm.hasModelInCache(MODEL_ID);
    } catch {
        return false;
    }
}

export async function load(dotnetRef) {
    if (engine) return;
    try {
        engine = await webllm.CreateMLCEngine(MODEL_ID, {
            initProgressCallback: (report) =>
                dotnetRef.invokeMethodAsync("OnProgress", report.progress, report.text),
        });
    } catch (e) {
        engine = null;
        const text = e?.message ?? String(e);
        throw new Error(text.includes("Device") || text.includes("GPU")
            ? "The graphics card ran out of memory. Close other tabs that use the AI and reload the page."
            : text);
    }
}

export async function ask(messages, dotnetRef) {
    if (!engine) throw new Error("The AI is not loaded yet.");
    const chunks = await engine.chat.completions.create({
        messages,
        stream: true,
        max_tokens: 500,
        temperature: 0.3,
    });
    let answer = "";
    for await (const chunk of chunks) {
        const piece = chunk.choices[0]?.delta?.content ?? "";
        if (piece) {
            answer += piece;
            await dotnetRef.invokeMethodAsync("OnToken", piece);
        }
    }
    return answer;
}
