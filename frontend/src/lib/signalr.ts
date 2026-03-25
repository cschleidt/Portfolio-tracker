"use client";

import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null;

export function getSignalRConnection(): signalR.HubConnection {
  if (!connection) {
    const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "";
    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiUrl}/hubs/notifications`)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }
  return connection;
}
