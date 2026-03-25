import { notFound } from "next/navigation";
import { api } from "@/lib/api";
import { PortfolioTable } from "@/components/PortfolioTable";
import { PerformanceBadge } from "@/components/PerformanceBadge";

export const revalidate = 60;

interface Props {
  params: { id: string };
}

export default async function InvestorPage({ params }: Props) {
  const [investor, portfolio] = await Promise.all([
    api.getInvestor(params.id).catch(() => null),
    api.getPortfolio(params.id).catch(() => null),
  ]);

  if (!investor) notFound();

  const formattedValue = investor.totalPortfolioValue.toLocaleString("da-DK", {
    style: "currency",
    currency: "DKK",
    maximumFractionDigits: 0,
  });

  return (
    <div className="space-y-8">
      <a href="/" className="inline-flex items-center gap-1 text-sm text-gray-500 hover:text-white transition-colors">
        ← All investors
      </a>

      <div className="flex items-start gap-5">
        {investor.imageUrl ? (
          <img
            src={investor.imageUrl}
            alt={investor.name}
            className="h-20 w-20 rounded-full object-cover border-2 border-gray-700 shrink-0"
          />
        ) : (
          <div className="flex h-20 w-20 shrink-0 items-center justify-center rounded-full bg-brand-900 text-brand-400 text-2xl font-bold border-2 border-brand-800">
            {investor.name.charAt(0)}
          </div>
        )}
        <div className="min-w-0">
          <div className="flex flex-wrap items-center gap-3">
            <h1 className="text-3xl font-bold text-white">{investor.name}</h1>
            <PerformanceBadge value={investor.performancePercent} size="lg" />
          </div>
          <p className="mt-2 text-gray-400 max-w-2xl">{investor.description}</p>
          <div className="mt-3 flex flex-wrap gap-6 text-sm">
            <span className="text-gray-500">
              Total value:{" "}
              <span className="text-white font-semibold">{formattedValue}</span>
            </span>
            <span className="text-gray-500">
              Holdings:{" "}
              <span className="text-white font-semibold">{investor.holdingsCount}</span>
            </span>
            <span className="text-gray-500">
              Last synced:{" "}
              <span className="text-white font-semibold">
                {new Date(investor.lastSyncedAt).toLocaleString("da-DK")}
              </span>
            </span>
          </div>
        </div>
      </div>

      <div>
        <h2 className="mb-4 text-xl font-semibold text-white">Holdings</h2>
        {portfolio ? (
          <PortfolioTable holdings={portfolio.holdings} />
        ) : (
          <div className="rounded-xl border border-gray-800 bg-gray-900 p-12 text-center text-gray-500">
            Portfolio data is being synced…
          </div>
        )}
      </div>
    </div>
  );
}
