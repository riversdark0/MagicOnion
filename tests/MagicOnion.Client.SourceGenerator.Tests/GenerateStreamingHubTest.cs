using MagicOnion.Client.SourceGenerator.CodeAnalysis;
using MagicOnion.Client.SourceGenerator.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace MagicOnion.Client.SourceGenerator.Tests;

public class GenerateStreamingHubTest
{
    [Fact]
    public async Task Complex()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver
            {
                void OnMessage();
                void OnMessage2(MyObject a);
                void OnMessage3(MyObject a, string b, int c);

            }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A();
                Task B(MyObject a);
                Task C(MyObject a, string b);
                Task D(MyObject a, string b, int c);
                Task<int> E(MyObject a, string b, int c);
            }

            [MessagePackObject]
            public class MyObject
            {
            }

            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task HubReceiver_Parameter_Zero()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver
            {
                void OnMessage();
            }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task HubReceiver_Parameter_One()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver
            {
                void OnMessage(MyObject arg0);
            }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task HubReceiver_Parameter_Many()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver
            {
                void OnMessage(MyObject arg0, int arg1, string arg2);
            }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Return_Task()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Return_TaskOfT()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task<MyObject> A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Return_ValueTask()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                ValueTask A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Return_ValueTaskOfT()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                ValueTask<MyObject> A(MyObject a);
            }

            [MessagePackObject]
            public class MyObject
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Return_Void()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                void A();
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Invalid_HubReceiver_ReturnsNotVoid()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver
            {
                int {|#0:B|}();
            }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        var verifierOptions = VerifierOptions.Default with
        {
            TestBehaviorsOverride = TestBehaviors.SkipGeneratedSourcesCheck,
            ExpectedDiagnostics = new[]
            {
                new DiagnosticResult(MagicOnionDiagnosticDescriptors.StreamingHubUnsupportedReceiverMethodReturnType.Id, DiagnosticSeverity.Error).WithLocation(0),
            }
        };
        await MagicOnionSourceGeneratorVerifier.RunAsync(source, verifierOptions: verifierOptions);
    }

    [Fact]
    public async Task Parameter_Zero()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A();
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Parameter_One()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(string arg0);
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task Parameter_Many()
    {
        var source = """
        using System;
        using System.Threading.Tasks;
        using MessagePack;
        using MagicOnion;
        using MagicOnion.Client;
        
        namespace TempProject
        {
            public interface IMyHubReceiver { }
            public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
            {
                Task A(string arg0, int arg1, bool arg2);
            }
        
            [MagicOnionClientGeneration(typeof(IMyHub))]
            partial class MagicOnionInitializer {}
        }
        """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task InterfaceInheritance()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver { }
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>, IExtraMethods2
                         {
                             Task A();
                         }
                         
                         public interface IExtraMethods
                         {
                             Task B();
                         }
                         
                         public interface IExtraMethods2 : IExtraMethods
                         {
                             Task C();
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task InterfaceInheritance_Receiver()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver : IExtraReceiverMethods2
                         {
                             void A();
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                         
                         public interface IExtraReceiverMethods
                         {
                             void B();
                         }
                         
                         public interface IExtraReceiverMethods2 : IExtraReceiverMethods
                         {
                             void C();
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Zero_NoReturnValue()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task A();
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_One_NoReturnValue()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task A(string arg1);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Many_NoReturnValue()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task A(string arg1, int arg2, bool arg3);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Zero()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A();
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_One()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A(string arg1);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Many()
    {
        var source = """
                     using System;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A(string arg1, int arg2, bool arg3);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Zero_With_Cancellation()
    {
        var source = """
                     using System;
                     using System.Threading;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A(CancellationToken cancellationToken);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_One_With_Cancellation()
    {
        var source = """
                     using System;
                     using System.Threading;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A(string arg1, CancellationToken cancellationToken);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }

    [Fact]
    public async Task ClientResult_Parameter_Many_With_Cancellation()
    {
        var source = """
                     using System;
                     using System.Threading;
                     using System.Threading.Tasks;
                     using MessagePack;
                     using MagicOnion;
                     using MagicOnion.Client;

                     namespace TempProject
                     {
                         public interface IMyHubReceiver
                         {
                             Task<string> A(string arg1, int arg2, bool arg3, CancellationToken cancellationToken);
                         }
                         
                         public interface IMyHub : IStreamingHub<IMyHub, IMyHubReceiver>
                         {
                         }
                     
                         [MagicOnionClientGeneration(typeof(IMyHub))]
                         partial class MagicOnionInitializer {}
                     }
                     """;

        await MagicOnionSourceGeneratorVerifier.RunAsync(source);
    }
}
