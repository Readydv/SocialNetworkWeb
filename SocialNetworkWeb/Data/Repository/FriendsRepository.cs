﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using SocialNetworkWeb.Models;
using System.Data.Entity;

namespace SocialNetworkWeb.Data.Repository
{
    public class FriendsRepository : Repository<Friend>
    {
        public FriendsRepository(ApplicationDbContext db) : base(db)
        {

        }

        public void AddFriend(User target, User Friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends == null)
            {
                var item = new Friend
                {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = Friend,
                    CurrentFriendId = Friend.Id,
                };

                Create(item);
            }
        }
        public List <User> GetFriendsByUser (User target)
        {
            var friends = Set
                .Include(x => x.CurrentFriend)
                .Include(x => x.User)
                .Where(x => x.User.Id == target.Id && x.CurrentFriend != null) // Добавляем проверку на null
                .Select(x => x.CurrentFriend);

            return friends.ToList();
        }

        public void DeleteFriend (User target, User Friend)
        {
            var friends = Set.AsEnumerable().FirstOrDefault(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends != null)
            {
                Delete(friends);
            }
        }

    }
}
