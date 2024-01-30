using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheGlacier2
{
    public class ColorRecord
    {
        public bool setColor = false;
        public static Color freezeColor;
        public static Color freezeColor_head;
        //融化渐变时间
        public const int meltTime_max = 20;
        public int meltTime = meltTime_max;
        public const int freezeTime_max = 80;
        public int freezeTime = freezeTime_max;//冻结时间
        public ColorRecord()
        {
            //冻结颜色
            freezeColor.r = 207.0f / 255.0f;
            freezeColor.g = 235.0f / 255.0f;
            freezeColor.b = 245.0f / 255.0f;
            freezeColor.a = 1.0f;

            freezeColor_head.r = 63.0f / 255.0f;
            freezeColor_head.g = 117.0f / 255.0f;
            freezeColor_head.b = 141.0f / 255.0f;
            freezeColor_head.a = 1.0f;
        }

        public void Step()
        {
            if (freezeTime > 0)
                freezeTime--;
            else if (meltTime > 0)
                meltTime--;
        }

        public void OneMoreFreeze()
        {
            meltTime = meltTime_max;
            freezeTime += freezeTime_max;
        }

        //渐变函数
        public static float Myfunc(float src, float now, float valueSrc, float valueNow)
        {
            return now - (now - src) * ((valueSrc - valueNow) / valueSrc);
        }

        public Color ColorChange(Color src, Color dst)
        {
            Color ret;
            ret.r = Myfunc(src.r, dst.r, meltTime_max, meltTime);
            ret.g = Myfunc(src.g, dst.g, meltTime_max, meltTime);
            ret.b = Myfunc(src.b, dst.b, meltTime_max, meltTime);
            ret.a = 1.0f;
            return ret;
        }
    }
}
    