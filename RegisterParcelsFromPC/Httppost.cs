using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RegisterParcelsFromPC
{
    class Httppost
    {
        public string token = "xoxb-2214954337954-2428290007717-2pMn0wGuBG4KDmudnV98t5Tx";
        public string channel = "Task";
        public string user_code;
        public string channel_code;
        public string message_str;//呼び出すときに作っておく　日本語でよい
        public void posting()
        {
            //文字コードを指定する
            System.Text.Encoding enc =
                System.Text.Encoding.GetEncoding("UTF-8");//slackを相手にすると、shift_jisだとだめだがUTF-8だと行ける

            //POST送信する文字列を作成
            string postData =
                $"token={token}&channel={channel}&text=test" +
                    System.Web.HttpUtility.UrlEncode(message_str, enc);
            //バイト型配列に変換
            byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);

            //WebRequestの作成
            System.Net.WebRequest req =
                System.Net.WebRequest.Create("https://slack.com/api/chat.postMessage");
            //メソッドにPOSTを指定
            req.Method = "POST";
            //ContentTypeを"application/x-www-form-urlencoded"にする
            req.ContentType = "application/x-www-form-urlencoded";
            //POST送信するデータの長さを指定
            req.ContentLength = postDataBytes.Length;

            //データをPOST送信するためのStreamを取得
            System.IO.Stream reqStream = req.GetRequestStream();
            //送信するデータを書き込む
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            //サーバーからの応答を受信するためのWebResponseを取得
            System.Net.WebResponse res = req.GetResponse();
            //応答データを受信するためのStreamを取得
            System.IO.Stream resStream = res.GetResponseStream();
            //受信して表示
            System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);

                //閉じる
                sr.Close();
            
        }
        public void posting_DM()
        {
            //DMに送るためには、DMのchannel_code(仮称）が必要。user_idとは別。
            //conversations.openにbotのtokenとuser_idをpostで送れば取得できる
            /*
             このサイトで試せる https://so-zou.jp/web-app/network/post/
             url https://slack.com/api/conversations.open
             entity body に以下を入力
                token=xoxb-2214954337954-2428290007717-2pMn0wGuBG4KDmudnV98t5Tx
                users=U026B4NDH0T
             */
            //公式ドキュメント
            //        https://api.slack.com/methods/conversations.open


            //文字コードを指定する
            System.Text.Encoding enc =
                System.Text.Encoding.GetEncoding("UTF-8");//slackを相手にすると、shift_jisだとだめだがUTF-8だと行ける


            //WebRequestの作成
            System.Net.WebRequest req =
                System.Net.WebRequest.Create($"https://slack.com/api/conversations.open");

            //POST送信する文字列を作成
            string postData =
                $"token={token}&users={user_code}";            
            //バイト型配列に変換
            byte[] postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
            //メソッドにPOSTを指定
            req.Method = "POST";
            //ContentTypeを"application/x-www-form-urlencoded"にする
            req.ContentType = "application/x-www-form-urlencoded";
            //POST送信するデータの長さを指定
            req.ContentLength = postDataBytes.Length;
            


            //データをPOST送信するためのStreamを取得
            System.IO.Stream reqStream = req.GetRequestStream();
            //送信するデータを書き込む
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            //サーバーからの応答を受信するためのWebResponseを取得
            System.Net.WebResponse res = req.GetResponse();
            //応答データを受信するためのStreamを取得
            System.IO.Stream resStream = res.GetResponseStream();
            //受信して表示
            System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
            string jsonstr_from_slack=sr.ReadToEnd();//{"ok":true,"no_op":true,"already_open":true,"channel":{"id":"D02CGGQABPG"}}
            //jsondicというdicに格納  一旦取り出せたことにする
              //var jsondic = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonstr_from_slack);
            //channel_code = jsondic["id"];
            channel_code = "D02CGGQABPG";
            //channel_code = "D02CP93LZFC";//向後君のやつ
            
            //↑これがconversations.openによるchannel_code取得の儀

            //↓これが実際のPOST

            //POST送信する文字列を作成
            string postData2 =
                $"token={token}&channel={channel_code}&text=" +
                    System.Web.HttpUtility.UrlEncode(message_str, enc);
            //バイト型配列に変換
            byte[] postData2Bytes = System.Text.Encoding.ASCII.GetBytes(postData2);

            //WebRequestの作成
            System.Net.WebRequest req2 =
                System.Net.WebRequest.Create("https://slack.com/api/chat.postMessage");
            //メソッドにPOSTを指定
            req2.Method = "POST";
            //ContentTypeを"application/x-www-form-urlencoded"にする
            req2.ContentType = "application/x-www-form-urlencoded";
            //POST送信するデータの長さを指定
            req2.ContentLength = postData2Bytes.Length;

            //データをPOST送信するためのStreamを取得
            System.IO.Stream req2Stream = req2.GetRequestStream();
            //送信するデータを書き込む
            req2Stream.Write(postData2Bytes, 0, postData2Bytes.Length);
            req2Stream.Close();

            //サーバーからの応答を受信するためのWebResponseを取得
            System.Net.WebResponse res2 = req2.GetResponse();
            //応答データを受信するためのStreamを取得
            System.IO.Stream res2Stream = res2.GetResponseStream();
            //受信して表示
            System.IO.StreamReader sr2 = new System.IO.StreamReader(res2Stream, enc);

            //閉じる
            sr.Close();



        }



    }
}
