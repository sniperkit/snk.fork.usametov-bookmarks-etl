/*
Sniperkit-Bot
- Status: analyzed
*/

﻿using Bookmarks.Common;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MongoDbImportUtil
{
    public static class MergeBookmarks
    {
        public static List<Bookmark> Merge(string bookmarksFile1, string bookmarksFile2)
        {            
            var bookmarks1 = JsonConvert.DeserializeObject<Bookmark[]>(File.ReadAllText(bookmarksFile1));
            var bookmarks2 = JsonConvert.DeserializeObject<Bookmark[]>(File.ReadAllText(bookmarksFile2));

            using (var md5 = MD5.Create())
            {
                //union all
                var result = bookmarks1.Union(bookmarks2, new EqualityComparer<Bookmark>(Equals));
                //group and merge
                return result.GroupBy(b => b.LinkUrl)
                          .Select(bg =>
                                    new Bookmark
                                    {
                                        Id = Utils.ComputeHash(bg.Key, md5)
                                        ,
                                        LinkUrl = bg.Key
                                        ,//concatenate descriptions
                                        Description = string.Join(" ... "
                                                                , bg.Select(b => b.Description ?? string.Empty)
                                                                    .Distinct().ToArray())
                                        ,//concatenate anchor text
                                        LinkText = string.Join(" ... "
                                                               , bg.Select(b => b.LinkText ?? string.Empty)
                                                                   .Distinct().ToArray())
                                        ,//merge tags
                                        Tags = bg.SelectMany(b => b.Tags).Distinct().ToList()
                                        ,
                                        AddDate = bg.Select(b => b.AddDate == DateTime.MinValue ? DateTime.Now : b.AddDate).Min()
                                    })
                          .ToList();
            }
        }

        private static bool Equals(Bookmark b1, Bookmark b2)
        {            
            var tag1 = new HashSet<string>(b1.Tags.Distinct());
            var tag2 = new HashSet<string>(b2.Tags.Distinct());
                        
            return (b1.LinkUrl.Equals(b2.LinkUrl)
                    && b1.LinkText.Equals(b2.LinkText)
                    && b1.Description.Equals(b2.Description)
                    && tag1.SetEquals(tag2));
        }
    }
}
