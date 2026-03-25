import type { Holding } from "@/types";
import { PerformanceBadge } from "./PerformanceBadge";

interface Props {
  holdings: Holding[];
}

export function PortfolioTable({ holdings }: Props) {
  const sorted = [...holdings].sort((a, b) => b.weightPercent - a.weightPercent);

  return (
    <div className="rounded-xl border border-gray-800 bg-gray-900 overflow-x-auto">
      <table className="w-full text-sm min-w-[700px]">
        <thead>
          <tr className="border-b border-gray-800 text-left">
            <th className="px-4 py-3 font-medium text-gray-500">Company</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Qty</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Entry</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Current</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Value</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Weight</th>
            <th className="px-4 py-3 font-medium text-gray-500 text-right">Return</th>
          </tr>
        </thead>
        <tbody>
          {sorted.map((h) => (
            <tr
              key={h.id}
              className="border-b border-gray-800/60 last:border-0 hover:bg-gray-800/40 transition-colors"
            >
              <td className="px-4 py-3">
                <span className="font-semibold text-white">{h.ticker}</span>
                <span className="ml-2 text-gray-400 text-xs">{h.companyName}</span>
              </td>
              <td className="px-4 py-3 text-right text-gray-300 tabular-nums">
                {h.quantity.toLocaleString("da-DK")}
              </td>
              <td className="px-4 py-3 text-right text-gray-300 tabular-nums">
                {h.entryPrice.toLocaleString("da-DK", { minimumFractionDigits: 2 })}
              </td>
              <td className="px-4 py-3 text-right text-gray-300 tabular-nums">
                {h.currentPrice.toLocaleString("da-DK", { minimumFractionDigits: 2 })}
                <span className="ml-1 text-xs text-gray-600">{h.currency}</span>
              </td>
              <td className="px-4 py-3 text-right font-semibold text-white tabular-nums">
                {h.marketValue.toLocaleString("da-DK", {
                  style: "currency",
                  currency: h.currency,
                  maximumFractionDigits: 0,
                })}
              </td>
              <td className="px-4 py-3 text-right text-gray-400 tabular-nums">
                {h.weightPercent.toFixed(1)}%
              </td>
              <td className="px-4 py-3 text-right">
                <PerformanceBadge value={h.changePercent} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
