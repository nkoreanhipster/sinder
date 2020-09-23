using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sinder
{
    public static class Converters
    {
        public static List<MatchUserDto> ConvertUserModelToMatchUserDto(List<UserModel> users)
        {
            List<MatchUserDto> l = new List<MatchUserDto>();

            foreach (UserModel u in users)
            {
                MatchUserDto m = new MatchUserDto()
                {
                    ID = u.ID,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Surname = u.Surname,
                    Location = u.Location,
                    Age = u.Age,
                    Gender = u.Gender,
                    Images = u.Images,
                    Interests = u.Interests
                };

                l.Add(m);
            }
            return l;
        }
    }
}
