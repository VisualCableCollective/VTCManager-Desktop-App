import React from 'react';
import type { AppProps } from 'next/app';

import '../styles/globals.css';
import {IpcContextProvider} from "../contexts/IpcContext";
import {TelemetryContextProvider} from "../contexts/TelemetryContext";

function MyApp({ Component, pageProps }: AppProps) {
  return (
    <React.Fragment>
      <IpcContextProvider>
          <TelemetryContextProvider>
              <Component {...pageProps} />
          </TelemetryContextProvider>
      </IpcContextProvider>
    </React.Fragment>
  )
}

export default MyApp
