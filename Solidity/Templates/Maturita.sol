// SPDX-License-Identifier: MIT
// Compatible with OpenZeppelin Contracts ^4.0.0
pragma solidity ^0.8.22;

import {ERC20} from "@openzeppelin/contracts/token/ERC20/ERC20.sol";
//import {ERC20Burnable} from "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import {ERC20Permit} from "@openzeppelin/contracts/token/ERC20/extensions/ERC20Permit.sol";
import {Owner} from "Maturita/Owner.sol";

contract Aexis is ERC20, ERC20Permit, Owner {

  //  uint256 private intialSupply = 111 * 10 ** decimals();

    uint8 public TransferFee;
    uint8 public BurnFee;
    address public FeeCollector = 0xAb8483F64d9C6d1EcF9b849Ae677dD3315835cb2;
    uint8 public StakingAPR;

    struct _Stake {
        uint256 amount;
        uint256 time;
    }

    mapping(address => _Stake) public AddressStaked;

    constructor(uint256 supply, uint8 decs)
        ERC20("Aexis Test", "AXS", supply, decs)
        ERC20Permit("Aexis Test")
        Owner(msg.sender)
    {
        _mint(msg.sender, supply);
    }

    function burn(uint256 amount) public onlyOwner {
        require(balanceOf(msg.sender) >= amount, "Not enough tokens to burn.");
        _burn(msg.sender, amount);
    }

    function mint(uint256 amount) public onlyOwner {
        _mint(msg.sender, amount);
    }

    function transfer(address to, uint256 amount) public override returns (bool) {
        require(balanceOf(msg.sender) >= amount, "Not enough tokens to send.");
        // 100 * 1) / 100 )
        uint256 burnTax = ((amount * BurnFee) / 100);
        uint256 transferTax = ((amount * TransferFee) / 100);
        uint256 _amount = amount - (burnTax + transferTax);
        _transfer(msg.sender, to, _amount);
        _transfer(msg.sender, FeeCollector, transferTax);
        burn(burnTax);
        return true;
    }

    // Do budoucna zmenit tak, aby uzivatele mohli kdykoliv stakovat vice tokenu, aniz by museli withdrawovat
    // Ted to ale takto musi byt, jelikoz by uzivatel mohl stakovat pouze jeden token napr, a po rekneme roce stakovat dalsich 1000 tokenu a pote ihned withdrawnout, 
    // a ziskal by odmenu jako, kdyby stakoval 1001 tokenu cely rok.
    function stake(uint256 amount) public returns (bool) {
        require(balanceOf(msg.sender) >= amount, "Not enough tokens to stake.");
         AddressStaked[msg.sender] = _Stake(amount, block.timestamp);
         _transfer(msg.sender, address(this), amount);
        return true;
    }

    // Zmenit v budoucnu z rocni odmeny na treba tydenni, nebo udelat nejakej algoritmus ze by to treba slo i po hodinne nebo realtime? Jestli to je vubec mozne.
    function withdraw(uint256 amount) public returns (bool){
         _Stake storage stake_ = AddressStaked[msg.sender]; // Retrieve the struct
        require(stake_.amount  > 0, "Not staked any tokens.");
        require(stake_.amount  >= amount, "Cannot withdraw more tokens than staked!");
        _transfer(address(this), msg.sender, stake_.amount);
        uint256 reward = amount * StakingAPR * (block.timestamp - stake_.time) / 3155692;
        AddressStaked[msg.sender] = _Stake(0, block.timestamp);
        mint(reward);
        return true;  
    }
}