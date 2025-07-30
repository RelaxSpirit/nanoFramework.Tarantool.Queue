[![NuGet](https://img.shields.io/nuget/dt/nf.Tarantool.Queue.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nf.Tarantool.Queue/)
# Welcome to the .NET nanoFramework nanoFramework.Tarantool.Queue repository

This repository contains the client library for working with [Tarantool.Queue](https://github.com/tarantool/queue/tree/master) a full-fledged message broker for .NET nanoFramework.

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Tarantool.Queue | [![Build Status](https://github.com/RelaxSpirit/nanoFramework.Tarantool.Queue/actions/workflows/nf.Tarantool.Queue_dotnet.yml/badge.svg?branch=main)](https://github.com/RelaxSpirit/nanoFramework.Tarantool.Queue/actions/) | [![NuGet](https://img.shields.io/nuget/v/nf.Tarantool.Queue.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nf.Tarantool.Queue/) |

# What can Tarantool.Queue for a microcontroller be useful for?
Using a message queue, you can:
1) Organize a distributed system of microcontrollers that share a common data bus.
2) Organize an event-based system for exchanging information from various sensors and microcontrollers.
3) Organize an archive of various system data and readings.
4) Use different types of message queues to customize the necessary information processing logic.

# Limitations
Due to the limited memory size in the microcontroller, the message in the queue cannot be too large. To create and delete queues, you need to connect to Tarantool.Queue with the appropriate permissions.

# Usage
The library supports all [Tarantool.Queue types](https://github.com/tarantool/queue/tree/master?tab=readme-ov-file#queue-types) and all [Queue module methods](https://github.com/tarantool/queue/tree/master?tab=readme-ov-file#using-the-queue-module).

Samples:

1) Taking a task from the queue ("consuming")
    ```csharp
     QueueClientOptions clientOptions = new QueueClientOptions($"{TarantoolHostIp}:3301");
     using (IQueue queue = TarantoolQueueContext.Instance.GetQueue(clientOptions))
     {
          ITube tube = queue["fifo_tube"];
          TubeTask? tubeTask = null;
          Type messageInstaceType = typeof(SamleMessage);
      
          while ((tubeTask = tybe.Take(messageInstaceType)) != null)
          {
              try
              {
                  //// Task processing....
                  //// .........
                  ////
  
                  //// Acknowledging the completion processing of a task
                  tube.Ack(messageInstaceType, tubeTask.TaskId);
              }
              catch(Exception ex)
              {
                  //// Burying a task. If necessary, bury the erroneous task.
                  tube.Bury(messageInstaceType, tubeTask.TaskId);
                  //// or
                  //// Delete task.
                  //// tube.Delete(messageInstaceType, tubeTask.TaskId);
  
                  Console.WriteLine(ex);
              }
              finaly
              {
                  tubeTask = null;
              }
          }
     }
   ```
3) Putting a task in a queue
   ```csharp
     QueueClientOptions clientOptions = new QueueClientOptions($"{TarantoolHostIp}:3301");
     using (IQueue queue = TarantoolQueueContext.Instance.GetQueue(clientOptions))
     {
          ITube tube = queue["fifo_tube"];
          var sampleMessage = new SamleMessage();
      
          try
          {
              tube.Put(sampleMessage);
          }
          catch(Exception ex)
          {
              Console.WriteLine(ex);
          }
     }
   ```


## Acknowledgements

The initial version of the nanoFramework.Tarantool.Queue library was coded by [Spirin Dmitriy](https://github.com/RelaxSpirit), who has kindly handed over the library to the .NET **nanoFramework** project.

## Feedback and documentation

For documentation, providing feedback, issues, and finding out how to contribute, please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** WebServer library is licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behaviour in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
