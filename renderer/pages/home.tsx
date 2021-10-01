import React from 'react';
import Link from 'next/link';
import Lottie from "lottie-react";
import TruckLoadingAnimation  from '../assets/lotties/truck-loading.json';

function Home() {
  return (
    <React.Fragment>
      <div className="w-screen h-screen bg-gray-900 flex items-center justify-center">
        <div className="max-w-lg">
          <Lottie animationData={TruckLoadingAnimation} />
          <Link href="/dashboard">
          <a>Go to dashboard</a>
          </Link>
        </div>
      </div>
    </React.Fragment>
  );
}

export default Home;
