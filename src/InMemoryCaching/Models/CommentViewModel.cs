﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryCaching.Models
{
    public class CommentViewModel
    {
        public string CaptchaCode { get; set; }
        internal string GenerateCaptchaCode(out string imgSrcData)
        {
            Bitmap objBMP = new System.Drawing.Bitmap(200, 60);
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBMP);

            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            //' Configure font to use for text
            Font objFont = new Font("Arial", 32, FontStyle.Bold);
            string randomStr = "";
            int[] myIntArray = new int[5];
            int x;

            //That is to create the random # and add it to our string
            Random autoRand = new Random();

            for (x = 0; x < 5; x++)
            {
                myIntArray[x] = System.Convert.ToInt32(autoRand.Next(0, 9));
                randomStr += (myIntArray[x].ToString());
            }

            //This is to add the string to session cookie, to be compared later
            //Session.Add("randomStr", randomStr);

            //' Write out the text
            objGraphics.Clear(Color.LightGray);
            HatchBrush hb = new HatchBrush(HatchStyle.Sphere, Color.Black, Color.LightGray);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
           // objGraphics.RotateTransform(10);
            objGraphics.DrawString(randomStr, objFont, hb, new RectangleF(0, 0, objBMP.Width, objBMP.Height), sf);
            objGraphics.DrawLine(Pens.Black, 0, 0, objBMP.Width, objBMP.Height - 1);
            objGraphics.DrawRectangle(Pens.Black, 0, 0, objBMP.Width - 1, objBMP.Height - 1);

            //' Set the content type and return the image
            MemoryStream ms = new MemoryStream();
            objBMP.Save(ms, ImageFormat.Gif);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            imgSrcData = "data:image/gif;base64," + base64Data;

            ms.Dispose();
            objFont.Dispose();
            objGraphics.Dispose();
            objBMP.Dispose();

            return randomStr;
        }
        internal string Validate(string originalCaptchaCode)
        {
            if (originalCaptchaCode != null && originalCaptchaCode != CaptchaCode)
                return "f";
            else
            return "t";
        }
        public List<Comment> Comments { get; set; }
        public List<decimal> decimalComm { get; set; }
    }
}
