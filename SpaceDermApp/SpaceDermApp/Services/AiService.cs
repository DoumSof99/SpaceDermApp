using Microsoft.JSInterop;

namespace SpaceDermApp.Services;

// One message in the chat, in the shape WebLLM expects.
public record ChatMessage(string role, string content);

// C# side of the in-browser AI. Calls wwwroot/js/ai.js through JS interop.
public class AiService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;
    private DotNetObjectReference<AiService>? _selfRef;

    public AiService(IJSRuntime js) => _js = js;

    public bool IsLoaded { get; private set; }

    // Raised while the model downloads/loads: progress 0..1 and a status text.
    public event Action<double, string>? ProgressChanged;

    // Raised for every new piece of the answer, so the page can show it live.
    public event Action<string>? TokenReceived;

    private async Task<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/ai.js");

    private DotNetObjectReference<AiService> SelfRef
        => _selfRef ??= DotNetObjectReference.Create(this);

    public async Task<bool> IsSupportedAsync()
        => await (await GetModuleAsync()).InvokeAsync<bool>("isSupported");

    public async Task LoadAsync()
    {
        if (IsLoaded) return;
        // The first load downloads ~700 MB, so allow plenty of time.
        await (await GetModuleAsync()).InvokeVoidAsync("load", TimeSpan.FromMinutes(30), SelfRef);
        IsLoaded = true;
    }

    public async Task<string> AskAsync(IEnumerable<ChatMessage> messages)
        => await (await GetModuleAsync()).InvokeAsync<string>("ask", TimeSpan.FromMinutes(5), messages, SelfRef);

    [JSInvokable]
    public void OnProgress(double progress, string text) => ProgressChanged?.Invoke(progress, text);

    [JSInvokable]
    public void OnToken(string piece) => TokenReceived?.Invoke(piece);

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); } catch (JSDisconnectedException) { }
        }
        _selfRef?.Dispose();
    }
}
