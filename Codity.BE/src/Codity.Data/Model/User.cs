﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Codity.Data.Model
{
    public class User: IdentityUser<int>, IBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AboutMe { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Image { get; set; }
       
        public int? GenderId { get; set; }
        public Gender Gender { get; set; }

        public Settings Settings { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<Follow> Following { get; set; }
        public ICollection<UserNotification> Notifications { get; set; }
    }
}
