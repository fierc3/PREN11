# PREN11-Backend
Backend Logic for PREN Project.


The backend manages, stores and verifies data sent by the robot.

## URLS

Testing: https://tactile-rigging-333212.oa.r.appspot.com

## How To Send Data to the server as c# client
For testing purposes the sending socket for the server doesn't require authentication. In the final project there will be authentication.

### C# Client

            await using var client = new SocketIoClient();
            client.Connected += (sender, args) => Console.WriteLine($"Connected: {args.Namespace}");
            client.Disconnected += (sender, args) => Console.WriteLine($"Disconnected. Reason: {args.Reason}, Status: {args.Status:G}");
            client.EventReceived += (sender, args) => Console.WriteLine($"EventReceived: Namespace: {args.Namespace}, Value: {args.Value}, IsHandled: {args.IsHandled}");
            client.HandledEventReceived += (sender, args) => Console.WriteLine($"HandledEventReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            client.UnhandledEventReceived += (sender, args) => Console.WriteLine($"UnhandledEventReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            client.ErrorReceived += (sender, args) => Console.WriteLine($"ErrorReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            client.ExceptionOccurred += (sender, args) => Console.WriteLine($"ExceptionOccurred: {args.Value}");
            await client.ConnectAsync(new Uri("https://tactile-rigging-333212.oa.r.appspot.com"));
            await client.Emit("robot", "This was sent via c#");
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            await client.DisconnectAsync();




