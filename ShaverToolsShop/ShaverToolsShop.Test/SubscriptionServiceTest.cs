﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ShaverToolsShop.Conventions.Enums;
using ShaverToolsShop.Conventions.Repositories;
using ShaverToolsShop.Conventions.Services;
using ShaverToolsShop.Entities;
using ShaverToolsShop.Services;

namespace ShaverToolsShop.Test
{
    [TestFixture]
    public class CalculateSubscriptionSum
    {
        [SetUp]
        public void SetUp()
        {
            _subscriptionReadRepository = new Mock<ISubscriptionReadRepository>();
            _subscriptionRepository = new Mock<ISubscriptionRepository>();
            _subscriptionService = new SubscriptionService(_subscriptionReadRepository.Object
                , _subscriptionRepository.Object);

            _subscription =
                new Subscription
                {
                    Id = Guid.Parse("0f19d0bc-1965-428c-a496-7b0cfa48c073"),
                    StartDate = DateTime.Parse("01/01/2017"),
                    EndDate = DateTime.Parse("03/01/2017"),
                    SubscriptionType = SubscriptionType.OnceInMonth,
                    Product =
                        new Product
                        {
                            Name = "Бритвенный станок",
                            Price = 1
                        }
                };

            _subscriptions = new List<Subscription> {_subscription};
        }

        private Mock<ISubscriptionReadRepository> _subscriptionReadRepository;
        private Mock<ISubscriptionRepository> _subscriptionRepository;
        private ISubscriptionService _subscriptionService;
        private List<Subscription> _subscriptions;
        private Subscription _subscription;

        [Test]
        public async Task ShouldAddSubscription_WhenWeAddSubscription()
        {
            //Arrange
            var startDate = DateTime.Parse("01/01/2017");
            _subscriptionRepository.Setup(m => m.AddNewSubscription(_subscription))
                .ReturnsAsync(_subscription);

            //Act
            var addedSubscription = await _subscriptionService.AddNewSubscription(_subscription, startDate);

            //Assert
            Assert.AreEqual(_subscription.Id, addedSubscription.Id);
        }

        [Test]
        public async Task ShouldReturnSubscriptionsWithProducts_WhenWeAskAllSubscriptionsWithProducts()
        {
            //Arrange
            _subscriptionReadRepository.Setup(x => x.GetAllSubscriptionsWithProducts()).ReturnsAsync(_subscriptions);

            //Act
            var result = await _subscriptionService.GetAllWithProducts();

            //Assert
            Assert.AreEqual(_subscriptions, result);
        }

        [Test]
        public async Task StartDateMustPassedDate_WhenWeAddSubscription()
        {
            //Arrange
            var startDate = DateTime.Parse("01/01/2017");
            _subscriptionRepository.Setup(m => m.AddNewSubscription(_subscription))
                .ReturnsAsync(_subscription);

            //Act
            var addedSubscription = await _subscriptionService.AddNewSubscription(_subscription, startDate);

            //Assert
            Assert.AreEqual(_subscription.StartDate, startDate);
        }

        [Test]
        public async Task SubscriptionChangesMustBeSaved_WhenWeStopedSubscription()
        {
            //Arrange
            var endDate = DateTime.Parse("03/01/2017");
            _subscriptionRepository.Setup(m => m.GetSubscriptionAsync(_subscription.Id)).ReturnsAsync(_subscription);


            //Act
            await _subscriptionService.StoppedSubscription(_subscription.Id, endDate);

            //Assert
            _subscriptionRepository.Verify(m => m.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task SubscriptionStatusMustBeStarted_WhenWeAddNewSubscription()
        {
            //Arrange
            var startDate = DateTime.Parse("01/01/2017");
            _subscriptionRepository.Setup(m => m.AddNewSubscription(_subscription)).ReturnsAsync(_subscription);

            //Act
            var addedSubscription = await _subscriptionService.AddNewSubscription(_subscription, startDate);

            //Assert
            Assert.AreEqual(SubscriptionStatus.Started, addedSubscription.SubscriptionStatus);
        }

        [Test]
        public void WeGetListWithFirtyOneDay_WhenWeAskDaysForSubscription()
        {
            //Arrange
           var daysInMonthList = new List<int>();
            for (var i = 0; i <= 31; i++)
            {
                daysInMonthList.Add(i);
            }

            //Act
            var newDaysInMonth = _subscriptionService.GetDaysInMonth();

            //Assert
            Assert.AreEqual(daysInMonthList, newDaysInMonth);
        }

        [Test]
        public async Task WeMustGetSubscriptionCost_WhenWeCalculatingAllSubscriptions()
        {
            //Arrange
            var reportDate = DateTime.Parse("02/01/2017");
            var subscriptionCost = 1;
            _subscriptionReadRepository.Setup(m => m.GetAllSubscriptionsWithProducts()).ReturnsAsync(_subscriptions);

            //Act
            var calculatedSubscriptionCost = await _subscriptionService.CalculateSubscriptionsCost(reportDate);

            //Assert
            Assert.AreEqual(subscriptionCost, calculatedSubscriptionCost);
        }
    }
}