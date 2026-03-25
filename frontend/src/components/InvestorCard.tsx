import Link from "next/link";
import type { Investor } from "@/types";
import { PerformanceBadge } from "./PerformanceBadge";

interface Props {
  investor: Investor;
}

export function InvestorCard({ investor }: Props) {
  return (
    <Link href={`/investors/${investor.id}`}>
      <div className="group rounded-xl border border-gray-800 bg-gray-900 p-5 hover:border-brand-600 hover:bg-gray-800 transition-all cursor-pointer h-full flex flex-col">
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-center gap-3 min-w-0">
            {investor.imageUrl ? (
              <img
                src={investor.imageUrl}
                alt={investor.name}
                className="h-11 w-11 shrink-0 rounded-full object-cover border border-gray-700"
              />
            ) : (
              <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-brand-900 text-brand-400 text-base font-bold border border-brand-800">
                {investor.name.charAt(0)}
              </div>
            )}
            <div className="min-w-0">
              <h3 className="font-semibold text-white group-hover:text-brand-400 transition-colors truncate">
                {investor.name}
              </h3>
              <p className="text-xs text-gray-500">{investor.holdingsCount} holdings</p>
            </div>
          </div>
          <PerformanceBadge value={investor.performancePercent} />
        </div>

        <p className="mt-3 text-sm text-gray-400 line-clamp-2 flex-1">
          {investor.description}
        </p>

        <div className="mt-4 pt-4 border-t border-gray-800">
          <p className="text-xs text-gray-500 mb-0.5">Portfolio value</p>
          <p className="text-lg font-bold text-white">
            {investor.totalPortfolioValue.toLocaleString("da-DK", {
              style: "currency",
              currency: "DKK",
              maximumFractionDigits: 0,
            })}
          </p>
        </div>
      </div>
    </Link>
  );
}
