using System;
using System.Configuration;
using Matrix.Sdk;
using Matrix.Sdk.Core.Domain.MatrixRoom;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Room.Create;
using Sodium;

namespace TeleTok
{
    public class MatrixListener
    {
        private static readonly CryptographyService CryptographyService = new();

        public record LoginRequest(Uri BaseAddress, string Username, string Password, string DeviceId);

        public async Task RunListener()
        {
            var factory = new MatrixClientFactory();
            var anotherFactory = new MatrixClientFactory();

            IMatrixClient client = factory.Create();
            IMatrixClient anotherClient = anotherFactory.Create();

            client.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (client.UserId != senderUserId)
                        TeleTok.LogMessage($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            anotherClient.OnMatrixRoomEventsReceived += (sender, eventArgs) =>
            {
                foreach (BaseRoomEvent roomEvent in eventArgs.MatrixRoomEvents)
                {
                    if (roomEvent is not TextMessageEvent textMessageEvent)
                        continue;

                    (string roomId, string senderUserId, string message) = textMessageEvent;
                    if (anotherClient.UserId != senderUserId)
                        TeleTok.LogMessage($"RoomId: {roomId} received message from {senderUserId}: {message}.");
                }
            };

            (Uri matrixNodeAddress, string username, string password, string deviceId) = CreateLoginRequest();
            await client.LoginAsync(matrixNodeAddress, username, password, deviceId);

            LoginRequest request2 = CreateLoginRequest();
            await anotherClient.LoginAsync(request2.BaseAddress, request2.Username, request2.Password, request2.DeviceId);

            if(client.IsLoggedIn)
            {
                TeleTok.LogMessage($"client.IsLoggedIn: {client.IsLoggedIn}");
                TeleTok.LogMessage($"client.IsSyncing: {client.IsSyncing}");
            }

            client.Start();
            anotherClient.Start();

            CreateRoomResponse createRoomResponse = await client.CreateTrustedPrivateRoomAsync(new[]
            {
                anotherClient.UserId
            });

            await anotherClient.JoinTrustedPrivateRoomAsync(createRoomResponse.RoomId);

            var spin = new SpinWait();
            while(anotherClient.JoinedRooms.Length ==0)
                spin.SpinOnce();

            await client.SendMessageAsync(createRoomResponse.RoomId, "Hello");
            await anotherClient.SendMessageAsync(anotherClient.JoinedRooms[0].Id, ", ");
            
            await client.SendMessageAsync(createRoomResponse.RoomId, "World");
            await anotherClient.SendMessageAsync(anotherClient.JoinedRooms[0].Id, "!");

            TeleTok.LogMessage($"client.IsLoggedIn: {client.IsLoggedIn}");
            TeleTok.LogMessage($"client.IsSyncing: {client.IsSyncing}");

            Console.ReadLine();

            client.Stop();
            anotherClient.Stop();
            
            Console.WriteLine($"client.IsLoggedIn: {client.IsLoggedIn}");
            Console.WriteLine($"client.IsSyncing: {client.IsSyncing}");

        }

        private static LoginRequest CreateLoginRequest()
        {
            var seed = Guid.NewGuid().ToString();
            KeyPair keyPair = CryptographyService.GenerateEd25519KeyPair(seed);

            byte[] loginDigest = CryptographyService.GenerateLoginDigest();
            string hexSignature = CryptographyService.GenerateHexSignature(loginDigest, keyPair.PrivateKey);
            string publicKeyHex = CryptographyService.ToHexString(keyPair.PublicKey);
            string hexId = CryptographyService.GenerateHexId(keyPair.PublicKey);

            var password = $"ed:{hexSignature}:{publicKeyHex}";
            string deviceId = publicKeyHex;
            
            LoginRequest loginRequest = new LoginRequest(TeleTok.mAddress, TeleTok.mBotUser, TeleTok.mBotPass, deviceId);

            return loginRequest;
        }
    }
}