"use client";

import { useEffect, useState } from "react";
import clsx from "clsx";
import type { PortfolioChange, ChangeType } from "@/types";
import { getSignalRConnection } from "@/lib/signalr";

interface Props {
  changes: PortfolioChange[];
}

const LABELS: Record<ChangeType, string> = {
  HoldingAdded: "Added",
  HoldingRemoved: "Removed",
  QuantityIncreased: "Increased",
  QuantityDecreased: "Decreased",
  PerformanceUpdated: "Updated",
};

const COLORS: Record<ChangeType, string> = {
  HoldingAdded: "bg-green-900/50 text-green-400 border-green-800",
  HoldingRemoved: "bg-red-900/50 text-red-400 border-red-800",
  QuantityIncreased: "bg-blue-900/50 text-blue-400 border-blue-800",
  QuantityDecreased: "bg-yellow-900/50 text-yellow-500 border-yellow-800",
  PerformanceUpdated: "bg-gray-800 text-gray-400 border-gray-700",
};

export function ChangesFeed({ changes: initial }: Props) {
  const [changes, setChanges] = useState(initial);

  useEffect(() => {
    const conn = getSignalRConnection();

    conn.on("PortfolioChanged", (change: PortfolioChange) => {
      setChanges((prev) => [change, ...prev].slice(0, 100));
    });

    if (conn.state === "Disconnected") {
      conn.start().catch(console.error);
    }

    return () => {
      conn.off("PortfolioChanged");
    };
  }, []);

  if (changes.length === 0) {
    return (
      <div className="rounded-xl border border-gray-800 bg-gray-900 p-10 text-center text-gray-500">
        No changes detected yet. The sync runs every 30 minutes.
      </div>
    );
  }

  return (
    <div className="space-y-2">
      {changes.map((change) => (
        <div
          key={change.id}
          className="flex items-start gap-3 rounded-lg border border-gray-800 bg-gray-900 px-4 py-3"
        >
          <span
            className={clsx(
              "shrink-0 rounded border px-2 py-0.5 text-xs font-medium",
              COLORS[change.changeType]
            )}
          >
            {LABELS[change.changeType]}
          </span>
          <div className="flex-1 min-w-0">
            <p className="text-sm text-gray-200">{change.description}</p>
            <p className="mt-0.5 text-xs text-gray-500">
              {change.investorName} · <span className="font-mono">{change.ticker}</span> ·{" "}
              {new Date(change.detectedAt).toLocaleString("da-DK")}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}
