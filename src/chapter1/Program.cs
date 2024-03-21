using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;



var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults();

hostBuilder.ConfigureAppConfiguration((context, config) =>
{
    config.AddUserSecrets<Program>();
});

hostBuilder.ConfigureServices(services =>
{
   services.AddSingleton<IKernel>(sp =>
    {
        IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
        string azureOpenAIKey ="";
        string azureOpenAIEndpoint = "";
        IKernel kernel = new KernelBuilder()
            .WithLogger(sp.GetRequiredService<ILogger<IKernel>>())
            .Configure(config => config.AddAzureChatCompletionService(
                deploymentName: "gpt-35-turbo",
                endpoint:azureOpenAIEndpoint,
                apiKey: azureOpenAIKey))
            .Build();

        return kernel; 
    });

    services.AddSingleton<IChatCompletion>(sp =>
    sp.GetRequiredService<IKernel>().GetService<IChatCompletion>());

    const string instructions = "You are a helpful friendly assistant.";
    services.AddSingleton<ChatHistory>(sp =>
        sp.GetRequiredService<IChatCompletion>().CreateNewChat(instructions));
});

IHost host = hostBuilder.Build();
host.Run();