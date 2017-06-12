﻿using System;
using Xunit;

namespace Xamarin.Forms.BehaviorsPack.Tests
{
    public class NotificationBehaviorFixture
    {
	    [Fact]
	    public void ReceiveEvent()
	    {
		    var behavior = new BehaviorMock {EventName = "TestEventA"};
		    var page = new PageMock();
			page.Behaviors.Add(behavior);

			var eventArgs = new EventArgs();
			page.RiseTestEventA(this, eventArgs);

			Assert.NotNull(behavior.Sender);
			Assert.Equal(this, behavior.Sender);
			Assert.NotNull(behavior.EventArgs);
			Assert.Equal(eventArgs, behavior.EventArgs);

		    behavior.Sender = null;
		    behavior.EventArgs = null;

			// ChangeEvent
		    behavior.EventName = "TestEventB";

			page.RiseTestEventA(this, eventArgs);
		    Assert.Null(behavior.Sender);
		    Assert.Null(behavior.EventArgs);

		    page.RiseTestEventB(this, eventArgs);
		    Assert.NotNull(behavior.Sender);
		    Assert.Equal(this, behavior.Sender);
		    Assert.NotNull(behavior.EventArgs);
		    Assert.Equal(eventArgs, behavior.EventArgs);

		    behavior.Sender = null;
		    behavior.EventArgs = null;
			page.Behaviors.Clear();

		    page.RiseTestEventB(this, eventArgs);
			Assert.Null(behavior.Sender);
		    Assert.Null(behavior.EventArgs);
		}

	    [Fact]
	    public void ReceiveEventAfterEventChanged()
	    {
		    var behavior = new BehaviorMock { EventName = "TestEventA" };
		    var page = new PageMock();
		    page.Behaviors.Add(behavior);


		    // ChangeEvent
		    behavior.EventName = "TestEventB";

		    var eventArgs = new EventArgs();
		    page.RiseTestEventA(this, eventArgs);
		    Assert.Null(behavior.Sender);
		    Assert.Null(behavior.EventArgs);

		    page.RiseTestEventB(this, eventArgs);
		    Assert.NotNull(behavior.Sender);
		    Assert.Equal(this, behavior.Sender);
		    Assert.NotNull(behavior.EventArgs);
		    Assert.Equal(eventArgs, behavior.EventArgs);
	    }

	    [Fact]
	    public void WhenNothingEventName()
	    {
		    var behavior = new BehaviorMock { EventName = "NothingEvent" };
		    var page = new PageMock();
		    var exception = Assert.Throws<NotImplementedException>(() => page.Behaviors.Add(behavior));

			Assert.NotNull(exception);
		}

		[Fact]
	    public void ReceiveInteragtionRequest()
	    {
		    var request = new NotificationRequest();
		    var behavior = new BehaviorMock { NotificationRequest = request};
		    var page = new PageMock();
		    page.Behaviors.Add(behavior);

		    request.Request(this);

			Assert.NotNull(behavior.Sender);
		    Assert.Equal(this, behavior.Sender);
		    Assert.NotNull(behavior.EventArgs);
		    Assert.Equal(EventArgs.Empty, behavior.EventArgs);

		    behavior.Sender = null;
		    behavior.EventArgs = null;
		    page.Behaviors.Clear();

		    request.Request(this);
		    Assert.Null(behavior.Sender);
		    Assert.Null(behavior.EventArgs);
	    }

	    [Fact]
	    public void ReceiveInteragtionRequestAfterInteractionRequestChanged()
	    {
		    var requestA = new NotificationRequest();
		    var behavior = new BehaviorMock { NotificationRequest = requestA };
		    var page = new PageMock();
		    page.Behaviors.Add(behavior);

			var requestB = new NotificationRequest();
		    behavior.NotificationRequest = requestB;

			requestA.Request(this);
		    Assert.Null(behavior.Sender);
		    Assert.Null(behavior.EventArgs);

			requestB.Request(this);

			Assert.NotNull(behavior.Sender);
		    Assert.Equal(this, behavior.Sender);
		    Assert.NotNull(behavior.EventArgs);
		    Assert.Equal(EventArgs.Empty, behavior.EventArgs);
	    }

	    [Fact]
	    public void NotifyReceivedEvent()
	    {
		    var request = new NotificationRequest();
			var behavior = new NotificationBehavior { NotificationRequest = request };

		    var receivedEvent = Assert.Raises<EventArgs>(h => behavior.Received += h, h => behavior.Received -= h, () => request.Request(this));

			Assert.NotNull(receivedEvent);
		    Assert.Equal(this, receivedEvent.Sender);
			Assert.Equal(EventArgs.Empty, receivedEvent.Arguments);
		}

	    private class EventArgsMock : EventArgs
	    {
		    
	    }

		private class PageMock : ContentPage
	    {
		    // ReSharper disable once EventNeverSubscribedTo.Local
		    public event EventHandler<EventArgs> TestEventA;
		    // ReSharper disable once EventNeverSubscribedTo.Local
		    public event EventHandler<EventArgs> TestEventB;
		    public void RiseTestEventA(object sender, EventArgs eventArgs)
		    {
				TestEventA?.Invoke(sender, eventArgs);
		    }
		    public void RiseTestEventB(object sender, EventArgs eventArgs)
		    {
			    TestEventB?.Invoke(sender, eventArgs);
		    }
	    }

		private class BehaviorMock : NotificationBehavior<Page, EventArgs>
	    {
		    public object Sender { get; set; }
		    public object EventArgs { get; set; }
		    protected override void OnReceived(object sender, EventArgs eventArgs)
		    {
			    Sender = sender;
			    EventArgs = eventArgs;
		    }
	    }
    }
}
