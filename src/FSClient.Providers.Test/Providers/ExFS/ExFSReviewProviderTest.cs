﻿namespace FSClient.Providers.Test.ExFS
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FSClient.Providers.Test.Helpers;
    using FSClient.Shared.Models;

    using NUnit.Framework;

    [TestFixture(Category = TestCategories.ReviewProvider)]
    public class ExFSReviewProviderTest
    {
        [OneTimeSetUp]
        public Task OneTimeSetUp()
        {
            return new ExFSSiteProvider(Test.ProviderConfigService).CheckIsAvailableAsync();
        }

        [TestCase("75013")]
        [Timeout(60000)]
        public async Task ExFSReviewsProvider_GetReviews(string id)
        {
            var siteProvider = new ExFSSiteProvider(Test.ProviderConfigService);
            var itemInfo = new ItemInfo(siteProvider.Site, id);

            var provider = new ExFSReviewProvider(siteProvider);

            var enumerable = provider.GetReviews(itemInfo);

            var reviews = await enumerable
                .Take(20)
                .ToListAsync()
                .ConfigureAwait(false);

            Assert.That(reviews.Count > 0, Is.True, "Zero reviews count");
            CollectionAssert.AllItemsAreNotNull(reviews, "Some item is null");
            CollectionAssert.AllItemsAreUnique(reviews, "Items in not unique");
            foreach (var (review, _) in reviews)
            {
                Assert.That(string.IsNullOrWhiteSpace(review.Id), Is.False, "Id is null or empty");
                Assert.That(review.Site, Is.EqualTo(provider.Site), $"Item site is invalid - {review.Id}");
                Assert.That(string.IsNullOrWhiteSpace(review.Reviewer), Is.False, $"Reviewer is null or empty - {review.Id}");
                Assert.That(string.IsNullOrWhiteSpace(review.Description), Is.False, $"Description is null or empty - {review.Id}");
                Assert.That(review.Date, Is.Not.Null, $"Date is null - {review.Id}");
                Assert.That(review.Date, Is.Not.EqualTo(default(DateTime)), $"Date is default - {review.Id}");
                Assert.That(review.Avatar, Is.Not.Null, $"Avatar is null - {review.Id}");
                Assert.That(review.Avatar!.IsAbsoluteUri, Is.True, $"Avatar is not absolute uri - {review.Id}");
            }
        }
    }
}
