
const Web3 = require("web3").default || require("web3");
const fs = require("fs");
const fetch = require("node-fetch");

const web3 = new Web3("http://geth-rpc:8545");

//const account = "0x59865b2aca42ae1c671d05a8cf5ebc42ca23f891";
const password = "12";

const abi = JSON.parse(fs.readFileSync("build/_app_MyToken_sol_MyToken.abi"));
const bytecode = fs.readFileSync("build/_app_MyToken_sol_MyToken.bin").toString();

//curl --location --request POST 'localhost:8545' \
//--header 'Content-Type: application/json' \
//--data-raw '{
//    "jsonrpc": "2.0",
//    "id": 3,
//    "method": "eth_accounts",
//    "params": []
//}'

miner_account = null;

(async () => {
	while (true) {

		const url = 'http://geth-rpc:8545';
		const data = {
			jsonrpc: "2.0",
			id: 3,
			method: "eth_accounts",
			params: []
		};

		const response = await fetch(url, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(data)
		});

		miner_account = (await response.json())['result'][0];

		console.log("miner_account:", miner_account);

		fs.writeFile('./miner_account.txt', miner_account, err => {
			if (err) { console.error(err); }
		});

		break;
	}
})();

(async () => {
	while (true) {
		try {
			await web3.eth.personal.unlockAccount(miner_account, password, 60);

			const contract = new web3.eth.Contract(abi);
			const deploy = contract.deploy({
				data: "0x" + bytecode,
				arguments: [web3.utils.toWei("1000", "ether")]
			});

			const gas = await deploy.estimateGas({ from: miner_account });
			const gasPrice = await web3.eth.getGasPrice(); // legacy gas price

			console.log("gasPrice: " + gasPrice + ", gas: " + gas);

			const receipt = await deploy.send({
				from: miner_account,
				gas,
				gasPrice
			});

			fs.writeFile('./contract.txt', receipt.options.address, err => {
				if (err) { console.error(err); }
			});

			console.log("Contract deployed at:", receipt.options.address);
			break; // exit loop on success
		} catch (e) {
			console.error("Deployment failed, retrying...", e);
			await new Promise(r => setTimeout(r, 2000)); // wait 2s before retry
		}
	}
})();
