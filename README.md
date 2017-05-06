# ScheduledAction  
---
[![NuGet](https://img.shields.io/nuget/v/M.ScheduledAction.svg)](https://www.nuget.org/packages/M.ScheduledAction)  

A wrapper around System.Threading.Timer for scheduling an action execution.  

### Usage  

*example:*  

    ISchedule schedule = new Daily(new TimeSpan(13, 30, 0));

    var options = new Options
    {
        ExecuteOnStart = true,
        OnReschedule = (s, t) => Console.WriteLine($"Next run at { DateTime.Now.Add(t) }"),
        OnError = (s, e) => Console.WriteLine($"Error: { e }"),
        AfterFailureSchedule = new Fixed(TimeSpan.FromMinutes(15))
    };

    Action action = () => { /* do something */ };

    IScheduledAction scheduledAction = new ScheduledAction(action, schedule, options);

    scheduledAction.Start();
    ...  
    scheduledAction.Stop();	 
  
### ISchedule  

Represents a sequence of events by getting the time interval to the next event every time `NextEventAfter()` is called.  
Current implementations:  
`Daily` - single event per day at given time.  
`RecurringSince` - recurring at given time interval since point in time.  
`Fixed` - every time returns fixed time interval.  
  
**Note:** Rescheduling is done after the action is executed.  
This means that `Fixed` schedule can 'slide' in time with the action execution times.  
For `RecurringSince` this means that if interval is shorter than execution time, some events can be missed.  
  
### Options  

Options (optional) can modify the ScheduledAction behavior and relay information.  

*ExecuteOnStart* - whether to execute the action on `Start()` regardless of the schedule.  
  
*AfterFailureSchedule* - alternative schedule to use when exception is thrown from the action. Used until first successfull execution, after which the 'normal' schedule is used again.  
  
*OnReschedule* - called (on a ThreadPool thread) when rescheduling occurs.  
  
*OnError* - called (on a ThreadPool thread) when exception during execution of the action occurs.  
  
### ScheduledAction  

Executes an action by given schedule.  
If `ISchedule.NextEventAfter()` returns negative `TimeSpan` the execution will be stopped and `IScheduleAction.Start()` must be called again.
 
