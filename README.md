# SpaceDermApp
Space Dermatology App

## Use of AI

We used AI tools, and we say where and how:

- **Code:** Claude (Anthropic, an LLM) generated most of the code in this repository: the Blazor pages, the C# services and the JavaScript that runs WebLLM. We gave the directions, chose the idea and the design, tested the app and decided on every change.
- **Research and text:** Claude helped us find and summarise NASA and medical sources and draft the text of the presentation and the submission. We checked the sources ourselves.
- **Inside the app:** the app uses an open-source language model, [Llama 3.2 1B Instruct](https://huggingface.co/meta-llama/Llama-3.2-1B-Instruct) by Meta, running offline in the browser through [WebLLM](https://github.com/mlc-ai/web-llm). It only suggests ideas to the crew doctor. It never gives a diagnosis; the doctor decides.
- **Images:** the illustrations in our presentation are AI-generated. Each one has a visible "AI-generated image" label on its slide. The slides were made with Gamma, an AI presentation tool.
- No NASA logos or branding were generated or modified.
