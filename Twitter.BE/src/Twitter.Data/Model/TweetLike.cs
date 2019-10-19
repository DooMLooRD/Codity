﻿namespace Twitter.Data.Model
{
    public class TweetLike: BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}