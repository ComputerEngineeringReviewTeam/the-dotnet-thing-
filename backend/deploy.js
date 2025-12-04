
const Web3 = require("web3").default || require("web3");
const fs = require("fs");

const web3 = new Web3("http://geth-rpc:8545");

const account = "0x59865b2aca42ae1c671d05a8cf5ebc42ca23f891";
const password = "12";

const abi = JSON.parse(fs.readFileSync("build/_app_MyToken_sol_MyToken.abi"));
const bytecode = fs.readFileSync("build/_app_MyToken_sol_MyToken.bin").toString();

(async () => {
	while (true) {
		try {
			await web3.eth.personal.unlockAccount(account, password, 60);

			const contract = new web3.eth.Contract(abi);
			const deploy = contract.deploy({
				data: "0x" + bytecode,
				arguments: [web3.utils.toWei("1000", "ether")]
			});

			const gas = await deploy.estimateGas({ from: account });
			const gasPrice = await web3.eth.getGasPrice(); // legacy gas price

			console.log("gasPrice: " + gasPrice + ", gas: " + gas);

			const receipt = await deploy.send({
				from: account,
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
