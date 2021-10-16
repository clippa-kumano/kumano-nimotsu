package com.example.top;

import android.os.AsyncTask;
import android.util.Log;

import java.io.IOException;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class OkHttpPost extends AsyncTask<String,String,String> {

    public static final MediaType JSON = MediaType.get("application/json; charset=utf-8");
    String json = "{\"name\":\"名前\", \"taxis\":\"分類\"}";

    @Override
    protected String doInBackground(String... strings) {

        OkHttpClient client = new OkHttpClient();

        //String url = "http://httpbin.org/post";
        String url = "http://192.168.100.13";

        RequestBody body = RequestBody.create(JSON, json);

        Request request = new Request.Builder()
                .url(url)
                .post(body)
                .build();

        try {
            Response response = client.newCall(request).execute();
            return response.body().string();
        } catch (IOException e) {
            e.printStackTrace();
        }

        return null;
    }

    @Override
    protected void onPostExecute(String str) {
        Log.d("Debug",str);
    }
}