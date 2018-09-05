using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BusinessIntegrationClient.Dtos;

namespace BusinessIntegrationClient
{
    public static class BusinessApiExtensions
    {
        /// <summary>
        ///     The Default Page Size for all ListAllXXXX methods.  If you specify a larger page size, it will still return 200.
        /// </summary>
        internal const int DefaultPageSize = 200;

        /// <summary>
        ///     A Predicate that always returns true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool DefaultFilter<T>(T value)
        {
            return true;
        }

        /// <summary>
        ///     List all items using paging.
        /// </summary>
        /// <typeparam name="T">the type of each item listed.</typeparam>
        /// <param name="pageSize">the max size of each page</param>
        /// <param name="readPage">A delegate that returns a single page of type T</param>
        /// <param name="filter">A Predicate that filters items</param>
        /// <returns></returns>
        /// <remarks>
        ///     Internally, the API will limit the # of results to 200 per page.
        ///     Use this to list all items using paging, and using an optional lambda that filters results.
        /// </remarks>
        private static List<T> ListAll<T>(int pageSize, Func<int, IList<T>> readPage, Predicate<T> filter)
        {
            if (readPage == null) throw new ArgumentNullException(nameof(readPage));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var pageIndex = 0;
            int pageRead;

            var items = new List<T>();
            do
            {
                var page = readPage(pageIndex++);

                pageRead = page.Count;

                items.AddRange(page.Where(item => filter(item)));
            } while (pageRead == pageSize);

            return items;
        }

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

        /// <summary>
        ///     Lists All Users that match the client side filter. This pages through all records to ensure all records are
        ///     returned.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="filter">A lambda that can filter results on the client side.</param>
        /// <returns></returns>
        public static List<UserSummary> ListAllUsers(this RestfulBusinessApiClient api,
            Predicate<UserSummary> filter = null)
        {
            var pageSize = DefaultPageSize;

            return ListAll(pageSize, pageIndex => api.ListUsers(pageIndex, pageSize),
                filter ?? DefaultFilter);
        }

        /// <summary>
        ///     Lists Users using pageIndex & pageSize. pageSize is limited to 200 results.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="pageIndex">current page index</param>
        /// <param name="pageSize">requested page size, under 200.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The server will limit pages to 200 records, using paging to get all records.
        /// </remarks>
        public static List<UserSummary> ListUsers(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<UserSummary>>("Users/" + query.ToQueryString()) ?? new List<UserSummary>();
        }

        public static User GetUser(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return api.GetJson<User>("Users/" + id.UrlEncode());
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

            return api.PutJson<User>("Users/" + user.UserName.UrlEncode(), user);
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

            return api.PostJson<User>("Users", user);
        }

        public static void DeleteUser(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            api.Delete("Users/" + id.UrlEncode());
        }

        /// <summary>
        ///     Lists All Assets that match the client side filter. This pages through all records to ensure all records are
        ///     returned.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="filter">A lambda that can filter results on the client side.</param>
        /// <returns></returns>
        public static List<Asset> ListAllAssets(this RestfulBusinessApiClient api, Predicate<Asset> filter = null)
        {
            var pageSize = DefaultPageSize;
            return ListAll(pageSize, pageIndex => api.ListAssets(pageIndex, pageSize), filter ?? DefaultFilter);
        }

        /// <summary>
        ///     Lists Assets using pageIndex & pageSize. pageSize is limited to 200 results.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="pageIndex">current page index</param>
        /// <param name="pageSize">requested page size, under 200.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The server will limit pages to 200 records, using paging to get all records.
        ///     <see cref="ListAllAssets" />
        /// </remarks>
        public static List<Asset> ListAssets(this RestfulBusinessApiClient api, int pageIndex = 0, int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<Asset>>("Assets/" + query.ToQueryString()) ?? new List<Asset>();
        }

        public static Asset GetAsset(this RestfulBusinessApiClient api, string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            return api.GetJson<Asset>("Assets/" + id.UrlEncode());
        }

        public static Asset PutAsset(this RestfulBusinessApiClient api, Asset asset)
        {
            return api.PutJson<Asset>("Assets/" + asset.Id.UrlEncode(), asset);
        }

        public static Asset PostAsset(this RestfulBusinessApiClient api, Asset asset)
        {
            return api.PostJson<Asset>("Assets", asset);
        }

        public static void DeleteAsset(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("Assets/" + id.UrlEncode());
        }

        /// <summary>
        /// Lists all Countries from the lookup table, using paging to ensure all records are returned.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<CountryInfo> ListAllCountries(this RestfulBusinessApiClient api, Predicate<CountryInfo> filter = null)
        {
            var pageSize = DefaultPageSize;
            return ListAll(pageSize, pageIndex => api.ListCountries(pageIndex, pageSize), filter ?? DefaultFilter);
        }

        /// <summary>
        ///     Lists Countries using pageIndex & pageSize. pageSize is limited to 200 results.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="pageIndex">current page index</param>
        /// <param name="pageSize">requested page size, under 200.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The server will limit pages to 200 records, using paging to get all records.
        /// </remarks>
        public static List<CountryInfo> ListCountries(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };


            return api.GetJson<List<CountryInfo>>("Countries/" + query.ToQueryString()) ?? new List<CountryInfo>();
        }

        public static List<ProfileInfo> ListProfiles(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<ProfileInfo>>("Profiles/" + query.ToQueryString()) ?? new List<ProfileInfo>();
        }

        public static List<EntityTypeInfo> ListEntityTypes(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<EntityTypeInfo>>("EntityTypes/" + query.ToQueryString()) ??
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

            return api.GetJson<List<StateInfo>>("States/" + query.ToQueryString()) ?? new List<StateInfo>();
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

            return api.GetJson<List<ConceptInfo>>("Concepts/" + query.ToQueryString()) ?? new List<ConceptInfo>();
        }

        public static List<ContactType> ListContactTypes(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<ContactType>>("ContactTypes/" + query.ToQueryString()) ?? new List<ContactType>();
        }

        /// <summary>
        ///     Lists All Retail Locations that match the client side filter. This pages through all records to ensure all records
        ///     are returned.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="filter">A lambda that can filter results on the client side.</param>
        /// <returns></returns>
        public static List<RetailLocationSummary> ListAllRetailLocations(
            this RestfulBusinessApiClient api,
            Predicate<RetailLocationSummary> filter = null)
        {
            var pageSize = DefaultPageSize;
            return ListAll(pageSize, pageIndex => api.ListRetailLocations(pageIndex, pageSize),
                filter);
        }


        /// <summary>
        ///     Lists Retail Locations using pageIndex & pageSize. pageSize is limited to 200 results.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="pageIndex">current page index</param>
        /// <param name="pageSize">requested page size, under 200.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The server will limit pages to 200 records, using paging to get all records.
        ///     <see cref="ListAllRetailLocations" />
        /// </remarks>
        public static List<RetailLocationSummary> ListRetailLocations(this RestfulBusinessApiClient api,
            int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<RetailLocationSummary>>("RetailLocations/" + query.ToQueryString()) ??
                   new List<RetailLocationSummary>();
        }

        public static RetailLocation GetRetailLocation(this RestfulBusinessApiClient api, string id)
        {
            return api.GetJson<RetailLocation>("RetailLocations/" + id.UrlEncode());
        }

        public static RetailLocation PutRetailLocation(this RestfulBusinessApiClient api,
            RetailLocation retailRetailLocation)
        {
            return api.PutJson<RetailLocation>("RetailLocations/" + retailRetailLocation.Id.UrlEncode(),
                retailRetailLocation);
        }

        public static RetailLocation PostRetailLocation(this RestfulBusinessApiClient api,
            RetailLocation retailRetailLocation)
        {
            return api.PostJson<RetailLocation>("RetailLocations", retailRetailLocation);
        }

        public static void DeleteRetailLocation(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("RetailLocations/" + id.UrlEncode());
        }

        /// <summary>
        ///     Lists All Restuarants that match that match the client side filter. This pages through all records to ensure all
        ///     records are returned.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<RestaurantSummary> ListAllRestaurants(this RestfulBusinessApiClient api,
            Predicate<RestaurantSummary> filter = null)
        {
            var pageSize = DefaultPageSize;
            return ListAll(pageSize, pageIndex => api.ListRestaurants(pageIndex, pageSize), filter ?? DefaultFilter);
        }

        /// <summary>
        ///     Lists Restaurants using pageIndex & pageSize. pageSize is limited to 200 results.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="pageIndex">current page index</param>
        /// <param name="pageSize">requested page size, under 200.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The server will limit pages to 200 records, using paging to get all records.
        ///     <see cref="ListAllRestaurants" />
        /// </remarks>
        public static List<RestaurantSummary> ListRestaurants(this RestfulBusinessApiClient api, int pageIndex = 0,
            int pageSize = -1)
        {
            var query = new NameValueCollection
            {
                {"pageIndex", pageIndex.ToString()},
                {"pageSize", pageSize.ToString()}
            };

            return api.GetJson<List<RestaurantSummary>>("Restaurants/" + query.ToQueryString()) ??
                   new List<RestaurantSummary>();
        }

        public static Restaurant GetRestaurant(this RestfulBusinessApiClient api, string id)
        {
            return api.GetJson<Restaurant>("Restaurants/" + id.UrlEncode());
        }

        public static Restaurant PutRestaurant(this RestfulBusinessApiClient api, Restaurant restaurant)
        {
            return api.PutJson<Restaurant>("Restaurants/" + restaurant.Id.UrlEncode(), restaurant);
        }

        public static Restaurant PostRestaurant(this RestfulBusinessApiClient api, Restaurant restaurant)
        {
            return api.PostJson<Restaurant>("Restaurants", restaurant);
        }

        public static void DeleteRestaurant(this RestfulBusinessApiClient api, string id)
        {
            api.Delete("Restaurants/" + id.UrlEncode());
        }
    }
}