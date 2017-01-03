﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ShaverToolsShop.Entities;

namespace ShaverToolsShop.Test
{
    [TestFixture]
    public class CalculateSubscriptionSum
    {
        private Mock<ISubscriptionReadRepository> _subscriptionReadRepository;
        private ISubscriptionService _subscriptionService;
        private List<Subscription> _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _subscriptionReadRepository = new Mock<ISubscriptionReadRepository>();
            _subscriptionService = new ISubscriptionService(_subscriptionReadRepository.Object);
            _subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    BeginDate = DateTime.Parse("01/01/2017"),
                    EndDate = DateTime.Parse("03/01/2017"),
                    Products = new List<Product>
                    {
                        new Product
                        {
                            Name = "Бритвенный станок",
                            Price = 1
                        },
                        new Product
                        {
                            Name = "Средство после бритья",
                            Price = 10
                        }
                    }
                }
            };
        }
        [Test]
        public async Task ShouldReturnSubscriptions_WhenWeAskAllSubscriptions()
        {
            //Arrange
            _subscriptionService.Setup(x => x.For<Subscription>()
                .ReturnsAsync(subscription));

            //Act
            List<Subscription> results = await _subscriptionService.GetAllAsync() as List<Subscription>;

            //Assert
            Assert.IsNotNull(results);
        }
    }
}