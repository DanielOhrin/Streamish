using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{

    public class VideoRepository : BaseRepository, IVideoRepository
    {
        private string SelectVideo(string alias)
        {
            if (alias != null)
            {
                return string.Format("{0}.Id VideoId, {0}.Title VideoTitle, {0}.Description VideoDescription, {0}.Url VideoUrl, {0}.DateCreated VideoDateCreated, {0}.UserProfileId UserProfileId", alias);
            }
            else
            {
                return "Id VideoId, Title VideoTitle, Description VideoDescription, Url VideoUrl, DateCreated VideoDateCreated, UserProfileId VideoUserProfileId";
            }
        }        
        private string SelectUserProfile(string alias)
        {
            if (alias != null)
            {
                return string.Format("{0}.[Name] UserProfileName, {0}.Email UserProfileEmail, {0}.DateCreated UserProfileDateCreated, {0}.ImageUrl UserProfileImageUrl", alias);
            }
            else
            {
                return "[Name] UserProfileName, Email UserProfileEmail, DateCreated UserProfileDateCreated, ImageUrl UserProfileImageUrl";
            }
        }

        public VideoRepository(IConfiguration configuration) : base(configuration) { }

        public List<Video> Search(string criterion, bool sortDescending)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                      SELECT {SelectVideo("v")}, {SelectUserProfile("up")}
                      FROM Video v 
                        JOIN UserProfile up ON v.UserProfileId = up.Id
                    ";

                    //! First space is important to keep from creating unintended words
                    if (criterion != null)
                    {
                        cmd.CommandText += " WHERE v.Title LIKE @Criterion OR v.Description LIKE @Criterion";
                        DbUtils.AddParameter(cmd, "@Criterion", criterion);
                    }

                    if (sortDescending)
                    {
                        cmd.CommandText += " ORDER BY v.DateCreated DESC";
                    }
                    else
                    {
                        cmd.CommandText += " ORDER BY v.DateCreated";
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Video> videos = new();

                        while (reader.Read())
                        {
                            videos.Add(new Video()
                            {
                                Id = DbUtils.GetInt(reader, "VideoId"),
                                Title = DbUtils.GetString(reader, "VideoTitle"),
                                Description = DbUtils.GetString(reader, "VideoDescription"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                Url = DbUtils.GetString(reader, "VideoUrl"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "UserProfileName"),
                                    Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                }
                            });
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> Since(DateTime? date)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                      SELECT {SelectVideo("v")}, {SelectUserProfile("up")}
                      FROM Video v 
                        JOIN UserProfile up ON v.UserProfileId = up.Id
                    ";

                    //! First space is important to keep from creating unintended words
                    date = date ?? DateTime.Today;

                    cmd.CommandText += " WHERE v.DateCreated >= @Date";
                    DbUtils.AddParameter(cmd, "@Date", date);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Video> videos = new();

                        while (reader.Read())
                        {
                            videos.Add(new Video()
                            {
                                Id = DbUtils.GetInt(reader, "VideoId"),
                                Title = DbUtils.GetString(reader, "VideoTitle"),
                                Description = DbUtils.GetString(reader, "VideoDescription"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                Url = DbUtils.GetString(reader, "VideoUrl"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "UserProfileName"),
                                    Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                }
                            });
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                    SELECT {SelectVideo("v")}, {SelectUserProfile("up")}
                    FROM Video v 
                        JOIN UserProfile up ON v.UserProfileId = up.Id
                    ORDER BY v.DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            videos.Add(new Video()
                            {
                                Id = DbUtils.GetInt(reader, "VideoId"),
                                Title = DbUtils.GetString(reader, "VideoTitle"),
                                Description = DbUtils.GetString(reader, "VideoDescription"),
                                Url = DbUtils.GetString(reader, "VideoUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "UserProfileName"),
                                    Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                },
                            });
                        }

                        return videos;
                    }
                }
            }
        }

        public List<Video> GetAllWithComments()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                    SELECT {SelectVideo("v")}, {SelectUserProfile("up")},
                        c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId,
                        cup.[Name] cupName, cup.Email cupEmail, cup.DateCreated cupDateCreated, cup.ImageUrl cupImageUrl
                    FROM Video v 
                        JOIN UserProfile up ON v.UserProfileId = up.Id
                        LEFT JOIN Comment c on c.VideoId = v.Id
                        LEFT JOIN UserProfile cup ON cup.Id = c.UserProfileId
                    ORDER BY  v.DateCreated
            ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var videos = new List<Video>();
                        while (reader.Read())
                        {
                            var videoId = DbUtils.GetInt(reader, "VideoId");

                            var existingVideo = videos.FirstOrDefault(p => p.Id == videoId);
                            if (existingVideo == null)
                            {
                                existingVideo = new Video()
                                {
                                    Id = videoId,
                                    Title = DbUtils.GetString(reader, "VideoTitle"),
                                    Description = DbUtils.GetString(reader, "VideoDescription"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "VideoUrl"),
                                    UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                    UserProfile = new UserProfile()
                                    {
                                        Id = DbUtils.GetInt(reader, "UserProfileId"),
                                        Name = DbUtils.GetString(reader, "UserProfileName"),
                                        Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    },
                                    Comments = new List<Comment>()
                                };

                                videos.Add(existingVideo);
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                existingVideo.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = videoId,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                    UserProfile = new UserProfile()
                                    {
                                        Id = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                        Name = DbUtils.GetString(reader, "cupName"),
                                        Email = DbUtils.GetString(reader, "cupEmail"),
                                        DateCreated = DbUtils.GetDateTime(reader, "cupDateCreated"),
                                        ImageUrl = DbUtils.GetString(reader, "cupImageUrl"),
                                    }
                                });
                            }
                        }

                        return videos;
                    }
                }
            }
        }

        public Video GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                          SELECT {SelectVideo("v")}, {SelectUserProfile("up")}
                          FROM Video v
                            LEFT JOIN UserProfile up ON up.Id = v.UserProfileId
                          WHERE v.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Video video = null;
                        if (reader.Read())
                        {
                            video = new Video()
                            {
                                Id = id,
                                Title = DbUtils.GetString(reader, "VideoTitle"),
                                Description = DbUtils.GetString(reader, "VideoDescription"),
                                DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                Url = DbUtils.GetString(reader, "VideoUrl"),
                                UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                UserProfile = new()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "UserProfileName"),
                                    Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated")
                                }

                            };
                        }

                        return video;
                    }
                }
            }
        } 
        
        public Video GetByIdWithComments(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                    SELECT {SelectVideo("v")}, {SelectUserProfile("up")},
                        c.Id AS CommentId, c.Message, c.UserProfileId AS CommentUserProfileId
                    FROM Video v 
                        JOIN UserProfile up ON v.UserProfileId = up.Id
                        LEFT JOIN Comment c on c.VideoId = v.id
                    WHERE v.Id = @Id
            ";


                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        Video video = null;

                        while (reader.Read())
                        {
                            if (video == null)
                            {
                                video = new Video()
                                {
                                    Id = id,
                                    Title = DbUtils.GetString(reader, "VideoTitle"),
                                    Description = DbUtils.GetString(reader, "VideoDescription"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    Url = DbUtils.GetString(reader, "VideoUrl"),
                                    UserProfileId = DbUtils.GetInt(reader, "UserProfileId"),
                                    UserProfile = new()
                                    {
                                        Id = DbUtils.GetInt(reader, "UserProfileId"),
                                        Name = DbUtils.GetString(reader, "UserProfileName"),
                                        Email = DbUtils.GetString(reader, "UserProfileEmail"),
                                        ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                        DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated")
                                    },
                                    Comments = new List<Comment>()
                                };
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                video.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "Message"),
                                    VideoId = id,
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId")
                                });
                            }
                        }

                        return video;
                    }
                }
            }
        }

        public void Add(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Video (Title, Description, DateCreated, Url, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (@Title, @Description, @DateCreated, @Url, @UserProfileId)";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);

                    video.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Video video)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Video
                           SET Title = @Title,
                               Description = @Description,
                               DateCreated = @DateCreated,
                               Url = @Url,
                               UserProfileId = @UserProfileId
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Title", video.Title);
                    DbUtils.AddParameter(cmd, "@Description", video.Description);
                    DbUtils.AddParameter(cmd, "@DateCreated", video.DateCreated);
                    DbUtils.AddParameter(cmd, "@Url", video.Url);
                    DbUtils.AddParameter(cmd, "@UserProfileId", video.UserProfileId);
                    DbUtils.AddParameter(cmd, "@Id", video.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Video WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}