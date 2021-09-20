package com.example.top;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteStatement;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;

import java.text.SimpleDateFormat;
import java.util.Date;

public class MainActivity extends AppCompatActivity {


    private DatabaseHelper _helper;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        ImageButton image_button_touroku = findViewById(R.id.image_button_touroku);
        TourokuListener listener = new TourokuListener();
        image_button_touroku.setOnClickListener(listener);

        Button kumanotouroku = findViewById(R.id.kumanotouroku);
        A101KumanoTourokuListener listener2 = new A101KumanoTourokuListener();
        kumanotouroku.setOnClickListener(listener2);

        // DBヘルパーオブジェクトを生成。
        _helper = new com.example.top.DatabaseHelper(MainActivity.this);
    }

    private class TourokuListener implements AdapterView.OnClickListener {
        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, Tabbed_Touroku.class);
            startActivity(intent);
        }
    }

    private class A101KumanoTourokuListener implements View.OnClickListener{
        @Override
        public void onClick(View view){

            // データベースヘルパーオブジェクトからデータベース接続オブジェクトを取得。
            SQLiteDatabase db = _helper.getWritableDatabase();

            // 日時情報を指定フォーマットの文字列で取得
            Date dateObj = new Date();
            SimpleDateFormat format = new SimpleDateFormat( "yyyy/MM/dd HH:mm:ss" );
            String date = format.format( dateObj );
            String ryousei = "A101KumanoAjiri";
            String mada ="MadaUketottenai";
            // インサート用SQL文字列の用意。
           // String sqlInsert = "INSERT INTO nimotsu (time, ryosei, done) VALUES (?, ?, ?)";
            String sqlInsert = "INSERT INTO nimotsu VALUES (?, ?, ?)";
            // SQL文字列を元にプリペアドステートメントを取得。
            SQLiteStatement stmt = db.compileStatement(sqlInsert);
            // 変数のバイド。
            stmt.bindString(1, date);
            stmt.bindString(2, ryousei);
            stmt.bindString(3, mada);
            // インサートSQLの実行。
            stmt.executeInsert();

            // 主キーによる検索SQL文字列の用意。
            String sql = "SELECT * FROM nimotsu ";
            // SQLの実行。
            Cursor cursor = db.rawQuery(sql, null);
            // データベースから取得した値を格納する変数の用意。データがなかった時のための初期値も用意。
            String note = "";
            // SQL実行の戻り値であるカーソルオブジェクトをループさせてデータベース内のデータを取得。
            while(cursor.moveToNext()) {
                // カラムのインデックス値を取得。
                int dateNote = cursor.getColumnIndex("time");
                // カラムのインデックス値を元に実際のデータを取得。
                note += cursor.getString(dateNote);
                int ryouseiNote =cursor.getColumnIndex("ryosei");
                note +=   cursor.getString(ryouseiNote);
                int ryouseiStatus =cursor.getColumnIndex("done");
                note +=   cursor.getString(ryouseiStatus);
               note += "\n";


            }
            // 感想のEditTextの各画面部品を取得しデータベースの値を反映。
            TextView etNote = findViewById(R.id.NimotuJoutai);
            etNote.setText(note);



        }
    }
}