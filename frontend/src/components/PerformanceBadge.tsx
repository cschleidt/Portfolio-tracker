import clsx from "clsx";

interface Props {
  value: number;
  size?: "sm" | "lg";
}

export function PerformanceBadge({ value, size = "sm" }: Props) {
  const isPositive = value >= 0;
  return (
    <span
      className={clsx(
        "inline-flex items-center rounded-full font-medium tabular-nums",
        size === "sm" ? "px-2 py-0.5 text-xs" : "px-3 py-1 text-sm",
        isPositive
          ? "bg-green-900/60 text-green-400 border border-green-800"
          : "bg-red-900/60 text-red-400 border border-red-800"
      )}
    >
      {isPositive ? "+" : ""}
      {value.toFixed(2)}%
    </span>
  );
}
