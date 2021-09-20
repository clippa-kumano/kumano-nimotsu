package com.example.top;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ImageButton;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        ImageButton image_button_touroku = findViewById(R.id.image_button_touroku);
        TourokuListener listener = new TourokuListener();
        image_button_touroku.setOnClickListener(listener);
    }

    private class TourokuListener implements AdapterView.OnClickListener {



        @Override
        public void onClick(View view) {
            Intent intent = new Intent(MainActivity.this, Tabbed_Touroku.class);
            startActivity(intent);
        }
    }
}