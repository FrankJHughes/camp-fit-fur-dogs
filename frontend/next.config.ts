import type { NextConfig } from 'next';

const isDev = process.env.NODE_ENV === 'development';

const nextConfig = {
  async rewrites() {
    if (!isDev) return [];

    return [
      {
        source: '/api/:path*',
        destination: 'http://localhost:5209/api/:path*',
      },
    ];
  },
};

export default nextConfig;
