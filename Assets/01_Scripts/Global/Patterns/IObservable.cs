
public interface IObservable
{
    string GetObservableType();
}

public interface IObserverSubscribe
{
    void OnNotify(IObservable observable, object data);
}