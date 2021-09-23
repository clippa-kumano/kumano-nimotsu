package com.example.top;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;


public class DatabaseHelper extends SQLiteOpenHelper {
    /**
     * データベースファイル名の定数フィールド。
     */
    private static final String DATABASE_NAME = "nimotsuApp.db";
    /**
     * バージョン情報の定数フィールド。
     */
    private static final int DATABASE_VERSION = 1;

    /**
     * コンストラクタ。
     */
    public DatabaseHelper(Context context) {
        // 親クラスのコンストラクタの呼び出し。
        super(context, DATABASE_NAME, null, DATABASE_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        // 寮生テーブル作成用SQL文字列の作成。
        StringBuilder sb_ryosei = new StringBuilder();
        sb_ryosei.append("CREATE TABLE ryosei (");
        sb_ryosei.append("_id INTEGER PRIMARY KEY,");
        sb_ryosei.append("block TEXT,");
        sb_ryosei.append("heya TEXT,");
        sb_ryosei.append("ryosei_name TEXT");
        sb_ryosei.append(");");
        String sql_ryosei = sb_ryosei.toString();
        // SQLの実行。
        db.execSQL(sql_ryosei);
        // 荷物テーブル作成用SQL文字列の作成。
        StringBuilder sb_nimotsu = new StringBuilder();
        sb_nimotsu.append("CREATE TABLE nimotsu (");
        sb_nimotsu.append("_id INTEGER PRIMARY KEY,");
        sb_nimotsu.append("time TEXT,");
        sb_nimotsu.append("block TEXT,");
        sb_nimotsu.append("heya TEXT,");
        sb_nimotsu.append("ryosei_name TEXT,");
        sb_nimotsu.append("done TEXT");
        sb_nimotsu.append(");");
        String sql_nimotsu = sb_nimotsu.toString();
   // SQLの実行。
        db.execSQL(sql_nimotsu);
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
    }
}