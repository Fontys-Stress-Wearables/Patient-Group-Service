﻿using NATS.Client;
using Patient_Group_Service.Models;

namespace Patient_Group_Service.Interfaces;

public interface INatsService
{
    public void Publish<T>(string topic, string tenantId, T data);
    public void Subscribe<T>(string target, Action<NatsMessage<T>> handler);
}