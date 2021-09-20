package com.example.top.ui.main;

import android.content.Context;

import androidx.annotation.Nullable;
import androidx.annotation.StringRes;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentPagerAdapter;

import com.example.top.Touroku_A1;
import com.example.top.Touroku_A2;
import com.example.top.Touroku_A3;
import com.example.top.R;

/**
 * A [FragmentPagerAdapter] that returns a fragment corresponding to
 * one of the sections/tabs/pages.
 */
public class SectionsPagerAdapter extends FragmentPagerAdapter {

    @StringRes
    private static final int[] TAB_TITLES = new int[]{
            R.string.tab_block_1,
            R.string.tab_block_2,
            R.string.tab_block_3,
            R.string.tab_block_4,
            R.string.tab_block_5,
            R.string.tab_block_6,
            R.string.tab_block_7,
            R.string.tab_block_8,
            R.string.tab_block_9,
            R.string.tab_block_10,

    };
    private final Context mContext;

    public SectionsPagerAdapter(Context context, FragmentManager fm) {
        super(fm);
        mContext = context;
    }

    @Override
    public Fragment getItem(int position) {
        // getItem is called to instantiate the fragment for the given page.
        // Return a PlaceholderFragment (defined as a static inner class below).
        Fragment fragment = null;
        switch (position){
            case 0:
                fragment = new Touroku_A1();
                break;
            case 1:
                fragment = new Touroku_A2();
                break;
            case 2:
                fragment = new Touroku_A3();

            case 3:
                fragment = new Touroku_A3();
            case 4:
                fragment = new Touroku_A3();
            case 5:
                fragment = new Touroku_A3();
            case 6:
                fragment = new Touroku_A3();
            case 7:
                fragment = new Touroku_A3();
            case 8:
                fragment = new Touroku_A3();
            case 9:
                fragment = new Touroku_A3();
            case 10:
                fragment = new Touroku_A3();


        }
        return fragment;

    }

    @Nullable
    @Override
    public CharSequence getPageTitle(int position) {
        return mContext.getResources().getString(TAB_TITLES[position]);
    }

    @Override
    public int getCount() {
        // Show 10 total pages.
        return 10;
    }
}