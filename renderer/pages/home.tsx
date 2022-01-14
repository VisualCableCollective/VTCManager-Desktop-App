import React, {useEffect, useState} from 'react';
import Link from 'next/link';
import Lottie from "lottie-react";
import Logo from "../public/img/vtcmanager_logo.png"
import {CgSpinnerAlt} from "react-icons/cg";
import {useIpc} from "../contexts/IpcContext";
import Popout from 'react-popout'
import {useRouter} from "next/router";
import {RiErrorWarningFill} from "react-icons/ri"

function Home() {
    const ipc = useIpc();
    const router = useRouter();

    const [statusMessage, setStatusMessage] = useState("Loading...");
    const [appVersion, setAppVersion] = useState("");
    const [errorDetailMessage, setErrorDetailMessage] = useState("");
    const [popout, setPopout] = useState(null);
    const [loadingFailed, setLoadingFailed] = useState(false);

    useEffect(() => {
        if(!ipc.ipcReady) {
            return;
        }

        ipc.ipcRenderer.on("app-version", (event, args) => {
            setAppVersion("v" + args.version);
        })

        ipc.ipcRenderer.on("init-finished", (event, args) => {
            router.push("/dashboard");
        })

        ipc.ipcRenderer.on('loading-status-update', (event, args) => {
            setStatusMessage(args.message);
        })

        ipc.ipcRenderer.on("show-loading-screen-error", (event, args) => {
            if (args.errorCode === "SERVICES_NOT_AVAILABLE") {
                setStatusMessage("Online services currently not available");
                setErrorDetailMessage("Unable to connect to the VTCManager services. Check your internet connection or try again later.");
                setLoadingFailed(true);
            }
        })

        ipc.ipcRenderer.send('init-app');
    }, [ipc.ipcReady]);

  return (
    <React.Fragment>
        {popout}
      <div className="w-screen h-screen bg-gray-900 flex items-center justify-center">
          {loadingFailed ? <LoadingErrorInfo ipc={ipc} statusMessage={statusMessage} /> : <LoadingInfo statusMessage={statusMessage} />}
          <div className="fixed bottom-0 right-0 mr-1 opacity-70 select-none">
              {appVersion}
          </div>
      </div>
    </React.Fragment>
  );
}

function LoadingInfo({statusMessage}) {
    return (
        <div className="">
            <div className="flex justify-center">
                <div className="relative flex justify-center" style={{width: "250px"}}><img src="/img/vtcmanager_logo.png" /></div>
            </div>
            <div className="text-3xl flex justify-center mt-8">
                <div className="flex items-center">
                    <CgSpinnerAlt className="animate-spin text-3xl mr-3" />
                    <h1 className="text-center font-bold text-2xl leading-none select-none">{statusMessage}</h1>
                </div>
            </div>
        </div>
    )
}

function LoadingErrorInfo({statusMessage, ipc}) {

    function quitApp() {
        ipc.ipcRenderer.send('quit-app');
    }

    function relaunchApp() {
        ipc.ipcRenderer.send('relaunch-app');
    }

    return (
        <div className="">
            <div className="flex justify-center">
                <div className="relative flex justify-center" style={{width: "250px"}}><img src="/img/vtcmanager_logo.png" /></div>
            </div>
            <div className="text-3xl flex items-center mt-8">
                <div>
                    <div className="flex items-center">
                        <RiErrorWarningFill className="text-3xl mr-2 text-red-500" />
                        <h1 className="text-center font-bold text-2xl leading-none select-none">{statusMessage}</h1>
                    </div>
                    <div className="flex justify-center mt-4">
                        <button className="bg-green-700 hover:bg-green-600 text-xl rounded px-3 py-2 mr-2" onClick={relaunchApp}>
                            <p className="leading-none">Relaunch</p>
                        </button>
                        <button className="bg-white bg-opacity-20 hover:bg-opacity-30 text-xl rounded px-3 py-2 ml-2" onClick={quitApp}>
                            <p className="leading-none">Quit</p>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default Home;
