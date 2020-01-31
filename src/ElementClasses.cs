using System;
using System.Collections.Generic;
using WebRenderer;

namespace WebHtml {
    public class WebElement {
        public HtmlNode Base;

        public WebElement(HtmlNode basenode) {
            Base = basenode;
        }
    }

    public class WebImage : WebElement {
        public int SizeX = 10;
        public int SizeY = 5;

        public class PixelData {
            public ColorEnum Color = ColorEnum.White;
            public char Character;

            public PixelData(char c) {
                Character = c;
            }
        }

        public List<List<PixelData>> ImageRows = new List<List<PixelData>>();

        public List<PixelData> GetRow(int row) {
            if (row >= ImageRows.Count)
                return null;

            return ImageRows[row];
        }

        public WebImage(HtmlNode Base) : base(Base) {
            for (int y = 0; y < SizeY; y++) {
                List<PixelData> row = new List<PixelData>();

                for (int x = 0; x < SizeX; x++) {
                    row.Add(new PixelData(' '));
                }

                ImageRows.Add(row);
            }

            ImageRows[1][2].Color = ColorEnum.Black;
            ImageRows[3][2].Color = ColorEnum.Black;

            ImageRows[0][7].Color = ColorEnum.Black;
            ImageRows[1][6].Color = ColorEnum.Black;
            ImageRows[2][5].Color = ColorEnum.Black;
            ImageRows[3][6].Color = ColorEnum.Black;
            ImageRows[4][7].Color = ColorEnum.Black;
        }
    }
}