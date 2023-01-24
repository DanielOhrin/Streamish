using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }

        public List<UserProfile> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], Email, ImageUrl, DateCreated FROM dbo.UserProfile";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserProfile> profiles = new();

                        while (reader.Read())
                        {
                            UserProfile newProfile = new()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetNullableString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };

                            profiles.Add(newProfile);
                        }

                        return profiles;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], Email, ImageUrl, DateCreated FROM dbo.UserProfile WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile profile = null;

                        if (reader.Read())
                        {
                            profile = new()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetNullableString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };

                        }

                        return profile;
                    }
                }
            }
        }        
        
        public UserProfile GetByFirebaseUserId(string firebaseUserId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, [Name], Email, ImageUrl, DateCreated FROM dbo.UserProfile WHERE FireBaseUserId = @firebaseUserId";

                    DbUtils.AddParameter(cmd, "@firebaseUserId", firebaseUserId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile profile = null;

                        if (reader.Read())
                        {
                            profile = new()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetNullableString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };

                        }

                        return profile;
                    }
                }
            }
        }

        public UserProfile GetByIdWithVideos(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT up.Id, up.[Name], up.Email, up.ImageUrl, up.DateCreated,
                               v.Id VideoId, v.Title VideoTitle, v.Description VideoDescription, v.Url VideoUrl, v.DateCreated VideoDateCreated,
                               c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId, c.VideoId CommentVideoId
                        FROM dbo.UserProfile up
                            LEFT JOIN dbo.Video v ON up.Id = v.UserProfileId
                            LEFT JOIN dbo.Comment c ON c.VideoId = v.Id
                        WHERE up.Id = @Id
                    ";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile profile = null;

                        while (reader.Read())
                        {
                            if (profile == null)
                            {
                                profile = new()
                                {
                                    Id = DbUtils.GetInt(reader, "Id"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    ImageUrl = DbUtils.GetNullableString(reader, "ImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                    Videos = new List<Video>()
                                };
                            }

                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                Video newVideo = new()
                                {
                                    Id = DbUtils.GetInt(reader, "VideoId"),
                                    Title = DbUtils.GetString(reader, "VideoTitle"),
                                    Description = DbUtils.GetString(reader, "VideoDescription"),
                                    Url = DbUtils.GetString(reader, "VideoUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    UserProfileId = DbUtils.GetInt(reader, "Id"),
                                    Comments = new List<Comment>()
                                };

                                profile.Videos.Add(newVideo);
                            }

                            var existingVideo = profile.Videos.FirstOrDefault(v => v.Id == DbUtils.GetInt(reader, "CommentVideoId"));
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            { 
                                existingVideo.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = DbUtils.GetInt(reader, "CommentVideoId"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }

                        return profile;
                    }
                }
            }
        }

        public void Add(UserProfile profile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO dbo.UserProfile (FireBaseUserId, [Name], Email, DateCreated, ImageUrl)
                        VALUES (@FireBaseUserId, @Name, @Email, @DateCreated, @ImageUrl)
                    ";

                    DbUtils.AddParameter(cmd, "@FireBaseUserId", profile.FirebaseUserId);
                    DbUtils.AddParameter(cmd, "@Name", profile.Name);
                    DbUtils.AddParameter(cmd, "@Email", profile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", profile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(UserProfile profile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE dbo.UserProfile
                        SET [Name] = @Name,
                            Email = @Email,
                            DateCreated = @DateCreated,
                            ImageUrl = @ImageUrl
                        WHERE Id = @Id
                    ";

                    DbUtils.AddParameter(cmd, "@Name", profile.Name);
                    DbUtils.AddParameter(cmd, "@Email", profile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", profile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", profile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", profile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM dbo.UserProfile
                        WHERE Id = @Id
                    ";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
