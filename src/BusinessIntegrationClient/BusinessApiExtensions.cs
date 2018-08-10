using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BusinessIntegrationClient.Dtos;

namespace BusinessIntegrationClient
{
    public static partial class BusinessApiExtensions
    {
        /// <summary>
        ///     Creates a query string from a <see cref="NameValueCollection" />.
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection nvc)
        {
            if (nvc == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                var values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (var value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }

        public static List<UserSummary> ListUsers(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<UserSummary>>("User/" + query.ToQueryString()) ?? new List<UserSummary>();
        }

        public static User GetUser(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return api.GetJson<User>("User/" + id.UrlEncode());
        }

        /// <summary>
        ///     Sends a PUT request to the server to UPDATE an existing User, using <see cref="User.UserName" /> as the resource
        ///     Id.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static User PutUser(this RestfulBusinessApiClient api, User user)
        {
            user.VerifyKeysAreValidXmlNames();

            return api.PutJson<User>("User/" + user.UserName.UrlEncode(), user);
        }

        /// <summary>
        ///     Sends a POST request to INSERT a new User.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static User PostUser(this RestfulBusinessApiClient api, User user)
        {
            user.VerifyKeysAreValidXmlNames();

            return api.PostJson<User>("User", user);
        }

        public static void DeleteUser(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException("id");
            api.Delete("User/" + id.UrlEncode());
        }

        public static List<Asset> ListAssets(this RestfulBusinessApiClient api, int pageIndex = 0, int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<Asset>>("Asset/" + query.ToQueryString()) ?? new List<Asset>();
        }

        public static Asset GetAsset(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException("id");

            return api.GetJson<Asset>("Asset/" + id.UrlEncode());
        }

        public static Asset PutAsset(this RestfulBusinessApiClient api, Asset asset)
        {
            return api.PutJson<Asset>("Asset/" + asset.Id.UrlEncode(), asset);
        }

        public static Asset PostAsset(this RestfulBusinessApiClient api, Asset asset)
        {
            return api.PostJson<Asset>("Asset", asset);
        }

        public static void DeleteAsset(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("Asset/" + id.UrlEncode());
        }

        public static List<CountryInfo> ListCountries(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };


            return api.GetJson<List<CountryInfo>>("Country/" + query.ToQueryString()) ?? new List<CountryInfo>();
        }

        public static List<ProfileInfo> ListProfiles(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<ProfileInfo>>("Profile/" + query.ToQueryString()) ?? new List<ProfileInfo>();
        }

        public static List<EntityTypeInfo> ListEntityTypes(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<EntityTypeInfo>>("EntityType/" + query.ToQueryString()) ??
                   new List<EntityTypeInfo>();
        }

        public static List<StateInfo> ListStates(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<StateInfo>>("State/" + query.ToQueryString()) ?? new List<StateInfo>();
        }

        public static List<StateInfo> ListStatesByCountry(this RestfulBusinessApiClient api, string countryCode,
            int pageIndex = 0, int pageSize = -1)
        {
            //url?
            //return api.GetJson<List<StateInfo>>("State/");
            return api.ListStates(pageIndex, pageSize)
                .Where(s => s.CountryCode == countryCode)
                .ToList();
        }


        public static List<ConceptInfo> ListConcepts(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<ConceptInfo>>("Concept/" + query.ToQueryString()) ?? new List<ConceptInfo>();
        }

        public static List<ContactType> ListContactTypes(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<ContactType>>("ContactType/" + query.ToQueryString()) ?? new List<ContactType>();
        }

        public static List<RetailLocationSummary> ListRetailLocations(this RestfulBusinessApiClient api,
            int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<RetailLocationSummary>>("RetailLocation/" + query.ToQueryString()) ??
                   new List<RetailLocationSummary>();
        }

        public static RetailLocation GetRetailLocation(this RestfulBusinessApiClient api, string id)
        {
            return api.GetJson<RetailLocation>("RetailLocation/" + id.UrlEncode());
        }

        public static RetailLocation PutRetailLocation(this RestfulBusinessApiClient api, RetailLocation retailRetailLocation)
        {
            return api.PutJson<RetailLocation>("RetailLocation/" + retailRetailLocation.Id.UrlEncode(), retailRetailLocation);
        }

        public static RetailLocation PostRetailLocation(this RestfulBusinessApiClient api, RetailLocation retailRetailLocation)
        {
            return api.PostJson<RetailLocation>("RetailLocation", retailRetailLocation);
        }

        public static void DeleteRetailLocation(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("RetailLocation/" + id.UrlEncode());
        }

        public static List<RestaurantSummary> ListRestaurants(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<RestaurantSummary>>("Restaurant/" + query.ToQueryString()) ?? new List<RestaurantSummary>();
        }

        public static Restaurant GetRestaurant(this RestfulBusinessApiClient api, string id)
        {
            return api.GetJson<Restaurant>("Restaurant/" + id.UrlEncode());
        }

        public static Restaurant PutRestaurant(this RestfulBusinessApiClient api, Restaurant restaurant)
        {
            return api.PutJson<Restaurant>("Restaurant/" + restaurant.Id.UrlEncode(), restaurant);
        }

        public static Restaurant PostRestaurant(this RestfulBusinessApiClient api, Restaurant restaurant)
        {
            return api.PostJson<Restaurant>("Restaurant", restaurant);
        }

        public static void DeleteRestaurant(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("Restaurant/" + id.UrlEncode());
        }

    }

}