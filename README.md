**Coroutine Manager for Unity**
A simple utility for managing coroutines in Unity. It allows you to track, start, and stop coroutines even when the sender (MonoBehaviour) is disabled, ensuring better control over delayed actions.

**Features**
Track and manage coroutines with unique IDs.
Continue coroutines even when the sender is disabled.
Clean up automatically once coroutines finish.
Supports both frame-based and time-based delays.
Add delayed callbacks using AddInvoke.
Efficiently manage multiple coroutines and invokes.

**Usage**
**Start a Coroutine:**
Guid coroutineId = CoroutineManager.Instance.StartManagedCoroutine(MyCoroutine()); // to run coroutine according to Unity lifecycle
Guid coroutineId = CoroutineManager.Instance.StartTrackedCoroutine(MyCoroutine()); // to track and manage coroutine

**Stop a Coroutine:**
CoroutineManager.Instance.StopCoroutine(coroutineId);
CoroutineManager.Instance.AbortAllCoroutines();

**Start an Invoke:**
Guid coroutineId = CoroutineManager.Instance.AddInvoke(duration, callback); // to start an invoke

**Cancel an Invoke:**
CoroutineManager.Instance.CancelInvoke(invokeId);
CoroutineManager.Instance.AbortAllInvokes();

**Installation**
Clone the repository.
Add CoroutineManager.cs to your Unity project.
Attach the CoroutineManager component to a GameObject.
