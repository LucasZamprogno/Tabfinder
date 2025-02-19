﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace TabFinder
{
    public class UGSearchResult
    {
        public string artist { get; }
        public string songAndVer { get; }
        public string href { get; }
        public int? numRatings { get; }
        public double? rating { get; }
        public bool canStoreContent { get; }
        public UGSearchResult(string artist, IWebElement div)
        {
            this.artist = artist;
            var columns = UGSearchResult.GetColumns(div);
            var nameCell = columns[1];
            var ratingCell = columns[2];
            var typeCell = columns[3];
            if (this.HasRating(ratingCell))
            {
                this.numRatings = this.GetNumRatings(ratingCell);
                this.rating = this.GetRating(ratingCell);
            }
            this.songAndVer = nameCell.Text.Trim();
            this.href = nameCell.FindElement(By.TagName("a")).GetProperty("href");
            this.canStoreContent = this.TextFormattedContent(typeCell);
        }

        public static bool PaidOnly(IWebElement div)
        {
            var columns = UGSearchResult.GetColumns(div);
            string type = columns[3].Text.Trim().ToLower();
            return type == "official" || type == "pro";
        }

        private bool HasRating(IWebElement ratingCell)
        {
            return ratingCell.FindElements(By.CssSelector("div > div")).Count > 0;
        }

        private double GetRating(IWebElement ratingCell) // background-position-x
        {
            var starSpans = ratingCell.FindElements(By.CssSelector("div > span"));
            double rating = 0;
            foreach (IWebElement star in starSpans)
            {
                string backgroundOffset = star.GetCssValue("background-position-x");
                if (backgroundOffset.StartsWith("0")) // Full star
                {
                    rating += 1;
                }
                else if (backgroundOffset.StartsWith("-16"))
                {
                    rating += 0.5;
                }
            }
            return rating;
        }

        private int GetNumRatings(IWebElement ratingCell)
        {
            try
            {
                string ratingString = ratingCell.Text.Trim();
                return Int32.Parse(ratingString);
            }
            catch (FormatException e)
            {
                return 0;
            }
        }

        private static List<IWebElement> GetColumns(IWebElement row)
        {
            var subDivs = row.FindElements(By.XPath("./*"));
            return new List<IWebElement>(subDivs);
        }

        private bool TextFormattedContent(IWebElement typeElem)
        {
            string type = typeElem.Text.ToLower().Trim();
            return !(type == "video" || type == "power" || type.Contains("pro"));
        }
    }
}
