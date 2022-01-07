import React, {createContext, useContext, useEffect, useState} from "react";
import electron, {IpcRenderer} from 'electron';

interface ContextProps {
    ipcRenderer: IpcRenderer,
    ipcReady: boolean,
}

const IpcContext = createContext<ContextProps>({
    ipcRenderer: null,
    ipcReady: false,
});

interface Props {
    children: React.ReactNode,
}
export function IpcContextProvider(props: Props) {
    const [ipcRenderer, setIpcRenderer] = useState<IpcRenderer>(null);
    const [ipcReady, setIpcReady] = useState(false);

    setTimeout(() => {
        if(!ipcRenderer) {
            const foundIpcRenderer = electron.ipcRenderer || false;
            if (foundIpcRenderer) {
                setIpcRenderer(foundIpcRenderer);
                setIpcReady(true);
            }
        }
    }, 500); // we have to wait otherwise a not working ipcRenderer will be used

    const context = {
        ipcRenderer: ipcRenderer,
        ipcReady,
    };

    return (
        <IpcContext.Provider value={context}>
            {props.children}
        </IpcContext.Provider>
    )
}

export const useIpc = () => useContext(IpcContext);
