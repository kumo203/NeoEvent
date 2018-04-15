using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NeoIvp_WebApp.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NeoIvp_WebApp.Controllers
{
#if DEBUG
    // when Debug build, [RequireHttps] will be not applied
#else
    // For now as beginning of development, disable RequireHttps
    // [RequireHttps]
#endif
    [Route("api")]
    public class CmsPlatformApiController : Controller
    {
        #region AccountAPI
        // Todo: Implement the code. Make a new wallet with Azure Face API's personId
        //       Reference Doc to create wallet: http://docs.neo.org/en-us/utility/sdk/common.html 

        #endregion

        #region EsateteAPI
        // Todo: Change the mock logic to actual
        public class EstatesArgument
        {
            public string personId { get; set; }
        }

        public class Estate
        {
            [JsonProperty("Name")]
            public string Name { get; set; }
            [JsonProperty("Owner")]
            public string Owner { get; set; }
            [JsonProperty("Status")]
            public string Status { get; set; }
            [JsonProperty("Country")]
            public string Country { get; set; }
            [JsonProperty("Area1")]
            public string Area1 { get; set; }
            [JsonProperty("Area2")]
            public string Area2 { get; set; }
            [JsonProperty("RoomType")]
            public string RoomType { get; set; }
            [JsonProperty("Grade")]
            public string Grade { get; set; }
            [JsonProperty("Taste")]
            public string Taste { get; set; }
            [JsonProperty("Evaluation")]
            public int Evaluation { get; set; }
            [JsonProperty("Comment")]
            public string[] Comment { get; set; }
            [JsonProperty("Introduction")]
            public string Inctroduction { get; set; }
            [JsonProperty("Facility")]
            public string Facility { get; set; }
            [JsonProperty("Access")]
            public string Access { get; set; }
        }

        public class EstateResultClass
        {
            [JsonProperty("Estate")]
            public Estate[] Estate { get; set; }
            public EstateResultClass() {
                // Set Mock Data for Test
                Estate = new Estate[] {
                    new Estate { Name = "Room1", Owner = "A", Status = "Open", Taste="洋風", Evaluation =5, Comment = new string[] { "◯◯さん(24歳)\r\n宿泊期間：2018/mm/dd\r\n友達ととても楽しい時間を過ごすことができました。\r\n近隣エリアに商業施設もあり、ショッピングの時間を作ること\r\nができました。夜はとても静かだったので、ゆっくりと休暇...", }, Grade = "G1", Country = "Japan", Area1 = "A1", Area2 = "A2", RoomType="2LDK", Access="・電車を利用するとき\r\n相鉄本線の西横浜駅より約5分(0.4km)\r\n・タクシーを利用するとき\r\n横浜駅東口のタクシーターミナルからタクシーがご利用いた...", Facility="神奈川県横浜市西区久保丁 ◯◯◯◯◯◯◯◯", Inctroduction="横浜駅、桜木町駅、馬車道駅にとても近い立地となります。みなとみらいにはショッピング施設やレストランも多く、家族で楽しい思い出を作ることができます。自宅は住宅街に位置しているため、夜はとても静かに過ごすことができます。" },
                    new Estate { Name = "Room1", Owner = "A", Status = "Open", Taste="洋風", Evaluation =5, Comment = new string[] { "Good", "Awesome" }, Grade = "G1", Country = "Japan", Area1 = "A1", Area2 = "A2", RoomType="2LDK", Access="Access1", Facility="Facility1", Inctroduction="Introduction1" },
                    new Estate { Name = "Room2", Owner = "A", Status = "NotAvailable", Taste="和風", Evaluation =4, Comment = new string[] { "Good", "So-So" }, Grade = "G2", Country = "Japan", Area1 = "A1", Area2 = "A2", RoomType="2LDK", Access="Access2", Facility="Facility2", Inctroduction="Introduction2" },
                    new Estate { Name = "Room3", Owner = "B", Status = "Borrowed", Taste="アジア風", Evaluation =3, Comment = new string[] { "Good", "Bad" }, Grade = "G2", Country = "US", Area1 = "A1", Area2 = "A2", RoomType="2LDK", Access="Access3", Facility="Facility3", Inctroduction="Introduction3" },
                };
            }

        }

        [HttpPost("Estates")]
        [Consumes("application/json")]
        public EstateResultClass EstatesJSON([FromBody]BorrowArgument argument)
        {
            return Estates(argument.PersonId, argument.Name);
        }
        [HttpPost("Estates")]
        [Consumes("application/x-www-form-urlencoded")]
        public EstateResultClass EstatesForm([FromForm]BorrowArgument argument)
        {
            return Estates(argument.PersonId, argument.Name);
        }
        public EstateResultClass Estates(string personId, string name)
        {
            return new EstateResultClass();
        }
        #endregion //EstatesAPI

        #region RoomCheckinApi
        static private readonly IFaceServiceClient faceServiceClient =
            new FaceServiceClient("c47f5b0cf3ea48f7b0a1ec1457f9a0d7",       // Azure Face API Key
            "https://westus2.api.cognitive.microsoft.com/face/v1.0");       // Azure Region Face API URL
        static readonly string PersonGroup = "samplegroup"; // Person Group Name
        static readonly int COUNTER_MAX = 100;              // Usage: 100ms*counter < COUNTER_MAX            

        public class BorrowArgument
        {
            public string PersonId { get; set; }
            public string Name { get; set; }
        }

        public class BorrowResultClass
        {
            [JsonProperty("Result")]
            public bool Result { get; set; }
            public BorrowResultClass()
            {
            }
        }

        [HttpPost("Borrow")]
        [Consumes("application/json")]
        public BorrowResultClass BorrowJSON([FromBody]BorrowArgument argument)
        {
            return Borrow(argument.PersonId, argument.Name);
        }
        [HttpPost("Borrow")]
        [Consumes("application/x-www-form-urlencoded")]
        public BorrowResultClass BorrowForm([FromForm]BorrowArgument argument)
        {
            return Borrow(argument.PersonId, argument.Name);
        }
        public BorrowResultClass Borrow(string personId, string name)
        {
            var list = faceServiceClient.ListPersonsInPersonGroupAsync(PersonGroup);
            int counter = 0;
            for (; counter<COUNTER_MAX; counter++)
            {
                if (list.IsCompleted)
                {
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
            if (counter >= COUNTER_MAX)
            {
                var fail = new BorrowResultClass();
                fail.Result = false;
                return fail;
            }

            if (personId==null)
            {
                var fail = new BorrowResultClass();
                fail.Result = false;
                return fail;
            }
            Guid personGuid = new Guid(personId);
            bool terminateFlag = true;
            foreach (var person in list.Result)
            {
                if (person.PersonId == personGuid)
                {
                    terminateFlag = false;
                    break;
                }
            }
            if (terminateFlag)
            {
                var fail = new BorrowResultClass();
                fail.Result = false;
                return fail;
            }

            var json = @"
{ 
 ""jsonrpc"": ""2.0"", 
 ""id"":119,
 ""method"": ""invokefunction"",
 ""params"":[
           ""0x6b5ebaf00d5627c0f9014df6fa3ff1432ce2e4a9"",
           ""Write"",
           [""" +  personId + @""", """ + name + @"""]]
}
";

            using (var client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = client.PostAsync(WConfig.GetInstance()["NeoPrivateNetUrl"], content);

                // Todo: Save result into Azure Storage
            }

            var ret = new BorrowResultClass
            {
                Result = true
            };
            return ret;
        }

        #endregion //RoomCheckinApi

        #region RoomCheckoutAPI
        // Todo: Change the mock logic to actual
        public class CheckoutArgument
        {
            public string PersonId { get; set; }
            public string Name { get; set; }
        }

        public class CheckoutResultClass
        {
            [JsonProperty("Result")]
            public bool Result { get; set; }
            [JsonProperty("Time")]
            public int Time { get; set; }
            [JsonProperty("Price")]
            public decimal Price { get; set; }
            public CheckoutResultClass()
            {
            }
        }

        [HttpPost("Checkout")]
        [Consumes("application/json")]
        public CheckoutResultClass CheckoutJSON([FromBody]CheckoutArgument argument)
        {
            return Checkout(argument.PersonId, argument.Name);
        }
        [HttpPost("Checkout")]
        [Consumes("application/x-www-form-urlencoded")]
        public CheckoutResultClass CheckoutForm([FromForm]CheckoutArgument argument)
        {
            return Checkout(argument.PersonId, argument.Name);
        }
        public CheckoutResultClass Checkout(string personId, string name)
        {
            // Currently Mock Value for Demo
            var ret = new CheckoutResultClass();
            ret.Result = true;
            ret.Time = 36;
            ret.Price = 100;

            return ret;
        }
        #endregion //RoomCheckoutAPI

    }
}