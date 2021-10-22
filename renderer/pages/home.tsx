import React, {useEffect, useState} from 'react';
import Link from 'next/link';
import Lottie from "lottie-react";
import Logo from "../public/img/vtcmanager_logo.png"
import {CgSpinnerAlt} from "react-icons/cg";
import {useIpc} from "../contexts/IpcContext";
import Popout from 'react-popout'
import {useRouter} from "next/router";

function Home() {
    const ipc = useIpc();
    const router = useRouter();

    const [statusMessage, setStatusMessage] = useState("Loading...");
    const [popout, setPopout] = useState(null);

    useEffect(() => {
        if(!ipc.ipcReady) {
            return;
        }

        ipc.ipcRenderer.send('init-app');

        ipc.ipcRenderer.on("init-finished", (event, args) => {
            router.push("/dashboard");
        })

        ipc.ipcRenderer.on('loading-status-update', (event, args) => {
            setStatusMessage(args.message);
        })
    }, [ipc.ipcReady]);

  return (
    <React.Fragment>
        {popout}
      <div className="w-screen h-screen bg-gray-900 flex items-center justify-center">
        <div className="">
            <div className="flex justify-center">
                <div className="relative flex justify-center" style={{width: "250px"}}><img src="/img/vtcmanager_logo.png" /></div>
            </div>
            <div className="text-3xl flex justify-center mt-8">
                <div className="flex items-center">
                    <CgSpinnerAlt className="animate-spin text-3xl mr-3" />
                    <h1 className="text-center font-bold text-2xl leading-none">{statusMessage}</h1>
                </div>
            </div>
        </div>
          <div className="fixed bottom-0 right-0 mr-1 opacity-70">
              v1.0.0
          </div>
      </div>
    </React.Fragment>
  );
}

export default Home;
