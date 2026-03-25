"use client";

import { useEffect, useState } from "react";
import type { PortfolioChange } from "@/types";
import { getSignalRConnection } from "@/lib/signalr";

export function useNotifications() {
  const [notifications, setNotifications] = useState<PortfolioChange[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    const conn = getSignalRConnection();

    conn.on("PortfolioChanged", (change: PortfolioChange) => {
      setNotifications((prev) => [change, ...prev].slice(0, 20));
      setUnreadCount((c) => c + 1);
    });

    if (conn.state === "Disconnected") {
      conn.start().catch(console.error);
    }

    return () => {
      conn.off("PortfolioChanged");
    };
  }, []);

  const markAllRead = () => setUnreadCount(0);

  return { notifications, unreadCount, markAllRead };
}
