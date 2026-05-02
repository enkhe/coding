// app/layout.tsx — root layout (server component)
import type { Metadata } from 'next';

export const metadata: Metadata = {
  title: { default: 'My App', template: '%s | My App' },
  description: 'Demo Next.js App Router site',
  openGraph: { type: 'website', siteName: 'My App' },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <header>
          <nav>
            <a href="/">Home</a> · <a href="/posts">Posts</a>
          </nav>
        </header>
        {children}
      </body>
    </html>
  );
}
