using System.Text.Json;
using System.Text;

public class PythonClientService {
    private readonly HttpClient _client;

    public PythonClientService(HttpClient client) {
        _client = client;
    }

    public async Task<string> GetBudgetAdviceAsync(BudgetAdviceDto dto) {
        var json = JsonSerializer.Serialize(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("http://localhost:7111/api/budget-advice", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
