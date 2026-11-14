// Runs a small AI model (Llama 3.2 1B) inside the browser with WebLLM.
// No server, no API key: the model is downloaded once (~700 MB) and cached by the browser.
// WebLLM: https://github.com/mlc-ai/web-llm
import * as webllm from "https://cdn.jsdelivr.net/npm/@mlc-ai/web-llm@0.2.88/+esm";

const MODEL_ID = "Llama-3.2-1B-Instruct-q4f16_1-MLC";
let engine = null;

// True if the browser can use the graphics card (WebGPU). Needed by WebLLM.
export async function isSupported() {
    if (!navigator.gpu) return false;
    try {
        return (await navigator.gpu.requestAdapter()) !== null;
    } catch {
        return false;
    }
}

// Loads the model and reports progress (0..1 + text) back to C#.
export async function load(dotnetRef) {
    if (engine) return;
    engine = await webllm.CreateMLCEngine(MODEL_ID, {
        initProgressCallback: (report) =>
            dotnetRef.invokeMethodAsync("OnProgress", report.progress, report.text),
    });
}

// Sends the conversation and streams the answer back to C# piece by piece.
// messages: [{ role: "system" | "user" | "assistant", content: "..." }]
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
