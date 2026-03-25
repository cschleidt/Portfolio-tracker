import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Portfolio Tracker – Millionaerklubben",
  description:
    "Track professional investors' portfolios from Saxo Bank's Millionaerklubben with real-time change notifications.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="da">
      <body className={`${inter.className} bg-gray-950 text-gray-100 min-h-screen`}>
        <nav className="sticky top-0 z-50 border-b border-gray-800 bg-gray-900/80 backdrop-blur px-6 py-4">
          <div className="mx-auto max-w-7xl flex items-center justify-between">
            <a href="/" className="text-xl font-bold text-white tracking-tight">
              Millionaer<span className="text-brand-500">Klubben</span>
            </a>
            <div className="flex gap-6 text-sm text-gray-400">
              <a href="/" className="hover:text-white transition-colors">
                Investors
              </a>
              <a href="/changes" className="hover:text-white transition-colors">
                Changes
              </a>
            </div>
          </div>
        </nav>
        <main className="mx-auto max-w-7xl px-4 sm:px-6 py-8">{children}</main>
      </body>
    </html>
  );
}
