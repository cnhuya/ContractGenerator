using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MovableForm;
using System.IO;
using System.Text.Json;
using cnsl;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;
using ContractGenerator.Properties;
using ContractGenerator;
using System.Web;

namespace ContractGenerator
{
    public partial class ContractGenerator : Form
    {
        public ContractGenerator()
        {
            InitializeComponent();
        }

        Console_Class _console = new Console_Class();

        private Color Scheme_Ligher;
        private Color Scheme_Darker;
        private Color Scheme_Text;

        private string Code;
        private string var;
        private string contract_type;

        public class SettingsData
        {
            public string Language { get; set; }
            public string Theme { get; set; }
            public Color HoverColor { get; set; } 
            public bool CloseOnStart { get; set; }
        }

        // zdroj: https://www.itnetwork.cz/csharp/kolekce-a-linq/c-sharp-tutorial-seznamy-kolekce-list
        private List<string> variables = new List<string>();
        private List<string> variablesInit = new List<string>();
        private List<string> variablesBuilder = new List<string>();

        private string name;
        private string symbol;
        private int decimals;
        private float supply;
        private string _supply;
        private float fee;
        private float burnfee;
        private float transferFee;
        private bool mintable;
        private bool burnable;
        private float stakingAPR;
        private string feeCollector;

        private string back;

        private string erc20 = "// SPDX-License-Identifier: MIT\r\n// OpenZeppelin Contracts (last updated v5.2.0) (token/ERC20/ERC20.sol)\r\n\r\npragma solidity ^0.8.20;\r\n\r\nimport {IERC20} from \"./IERC20.sol\";\r\nimport {IERC20Metadata} from \"./extensions/IERC20Metadata.sol\";\r\nimport {Context} from \"../../utils/Context.sol\";\r\nimport {IERC20Errors} from \"../../interfaces/draft-IERC6093.sol\";\r\n\r\n/**\r\n * @dev Implementation of the {IERC20} interface.\r\n *\r\n * This implementation is agnostic to the way tokens are created. This means\r\n * that a supply mechanism has to be added in a derived contract using {_mint}.\r\n *\r\n * TIP: For a detailed writeup see our guide\r\n * https://forum.openzeppelin.com/t/how-to-implement-erc20-supply-mechanisms/226[How\r\n * to implement supply mechanisms].\r\n *\r\n * The default value of {decimals} is 18. To change this, you should override\r\n * this function so it returns a different value.\r\n *\r\n * We have followed general OpenZeppelin Contracts guidelines: functions revert\r\n * instead returning `false` on failure. This behavior is nonetheless\r\n * conventional and does not conflict with the expectations of ERC-20\r\n * applications.\r\n */\r\nabstract contract ERC20 is Context, IERC20, IERC20Metadata, IERC20Errors {\r\n    mapping(address account => uint256) private _balances;\r\n\r\n    mapping(address account => mapping(address spender => uint256)) private _allowances;\r\n\r\n    uint256 private _totalSupply;\r\n\r\n    uint256 public burned;\r\n    uint256 public minted;\r\n    uint8 public _decimals;\r\n    string private _name;\r\n    string private _symbol;\r\n\r\n    /**\r\n     * @dev Sets the values for {name} and {symbol}.\r\n     *\r\n     * All two of these values are immutable: they can only be set once during\r\n     * construction.\r\n     */\r\n    constructor(string memory name_, string memory symbol_, uint8 decimals_) {\r\n        _name = name_;\r\n        _symbol = symbol_;\r\n        _decimals = decimals_;\r\n    }\r\n\r\n    /**\r\n     * @dev Returns the name of the token.\r\n     */\r\n    function name() public view virtual returns (string memory) {\r\n        return _name;\r\n    }\r\n\r\n    /**\r\n     * @dev Returns the symbol of the token, usually a shorter version of the\r\n     * name.\r\n     */\r\n    function symbol() public view virtual returns (string memory) {\r\n        return _symbol;\r\n    }\r\n\r\n    /**\r\n     * @dev Returns the number of decimals used to get its user representation.\r\n     * For example, if `decimals` equals `2`, a balance of `505` tokens should\r\n     * be displayed to a user as `5.05` (`505 / 10 ** 2`).\r\n     *\r\n     * Tokens usually opt for a value of 18, imitating the relationship between\r\n     * Ether and Wei. This is the default value returned by this function, unless\r\n     * it's overridden.\r\n     *\r\n     * NOTE: This information is only used for _display_ purposes: it in\r\n     * no way affects any of the arithmetic of the contract, including\r\n     * {IERC20-balanceOf} and {IERC20-transfer}.\r\n     */\r\n    function decimals() public view virtual returns (uint8) {\r\n        return _decimals;\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-totalSupply}.\r\n     */\r\n    function totalSupply() public view virtual returns (uint256) {\r\n        return _totalSupply;\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-balanceOf}.\r\n     */\r\n    function balanceOf(address account) public view virtual returns (uint256) {\r\n        return _balances[account];\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-transfer}.\r\n     *\r\n     * Requirements:\r\n     *\r\n     * - `to` cannot be the zero address.\r\n     * - the caller must have a balance of at least `value`.\r\n     */\r\n    function transfer(address to, uint256 value) public virtual returns (bool) {\r\n        address owner = _msgSender();\r\n        _transfer(owner, to, value);\r\n        return true;\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-allowance}.\r\n     */\r\n    function allowance(address owner, address spender) public view virtual returns (uint256) {\r\n        return _allowances[owner][spender];\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-approve}.\r\n     *\r\n     * NOTE: If `value` is the maximum `uint256`, the allowance is not updated on\r\n     * `transferFrom`. This is semantically equivalent to an infinite approval.\r\n     *\r\n     * Requirements:\r\n     *\r\n     * - `spender` cannot be the zero address.\r\n     */\r\n    function approve(address spender, uint256 value) public virtual returns (bool) {\r\n        address owner = _msgSender();\r\n        _approve(owner, spender, value);\r\n        return true;\r\n    }\r\n\r\n    /**\r\n     * @dev See {IERC20-transferFrom}.\r\n     *\r\n     * Skips emitting an {Approval} event indicating an allowance update. This is not\r\n     * required by the ERC. See {xref-ERC20-_approve-address-address-uint256-bool-}[_approve].\r\n     *\r\n     * NOTE: Does not update the allowance if the current allowance\r\n     * is the maximum `uint256`.\r\n     *\r\n     * Requirements:\r\n     *\r\n     * - `from` and `to` cannot be the zero address.\r\n     * - `from` must have a balance of at least `value`.\r\n     * - the caller must have allowance for ``from``'s tokens of at least\r\n     * `value`.\r\n     */\r\n    function transferFrom(address from, address to, uint256 value) public virtual returns (bool) {\r\n        address spender = _msgSender();\r\n        _spendAllowance(from, spender, value);\r\n        _transfer(from, to, value);\r\n        return true;\r\n    }\r\n\r\n    /**\r\n     * @dev Moves a `value` amount of tokens from `from` to `to`.\r\n     *\r\n     * This internal function is equivalent to {transfer}, and can be used to\r\n     * e.g. implement automatic token fees, slashing mechanisms, etc.\r\n     *\r\n     * Emits a {Transfer} event.\r\n     *\r\n     * NOTE: This function is not virtual, {_update} should be overridden instead.\r\n     */\r\n    function _transfer(address from, address to, uint256 value) internal {\r\n        if (from == address(0)) {\r\n            revert ERC20InvalidSender(address(0));\r\n        }\r\n        if (to == address(0)) {\r\n            revert ERC20InvalidReceiver(address(0));\r\n        }\r\n        _update(from, to, value);\r\n    }\r\n\r\n    /**\r\n     * @dev Transfers a `value` amount of tokens from `from` to `to`, or alternatively mints (or burns) if `from`\r\n     * (or `to`) is the zero address. All customizations to transfers, mints, and burns should be done by overriding\r\n     * this function.\r\n     *\r\n     * Emits a {Transfer} event.\r\n     */\r\n    function _update(address from, address to, uint256 value) internal virtual {\r\n        if (from == address(0)) {\r\n            // Overflow check required: The rest of the code assumes that totalSupply never overflows\r\n            _totalSupply += value;\r\n        } else {\r\n            uint256 fromBalance = _balances[from];\r\n            if (fromBalance < value) {\r\n                revert ERC20InsufficientBalance(from, fromBalance, value);\r\n            }\r\n            unchecked {\r\n                // Overflow not possible: value <= fromBalance <= totalSupply.\r\n                _balances[from] = fromBalance - value;\r\n            }\r\n        }\r\n\r\n        if (to == address(0)) {\r\n            unchecked {\r\n                // Overflow not possible: value <= totalSupply or value <= fromBalance <= totalSupply.\r\n                _totalSupply -= value;\r\n            }\r\n        } else {\r\n            unchecked {\r\n                // Overflow not possible: balance + value is at most totalSupply, which we know fits into a uint256.\r\n                _balances[to] += value;\r\n            }\r\n        }\r\n\r\n        emit Transfer(from, to, value);\r\n    }\r\n\r\n    /**\r\n     * @dev Creates a `value` amount of tokens and assigns them to `account`, by transferring it from address(0).\r\n     * Relies on the `_update` mechanism\r\n     *\r\n     * Emits a {Transfer} event with `from` set to the zero address.\r\n     *\r\n     * NOTE: This function is not virtual, {_update} should be overridden instead.\r\n     */\r\n    function _mint(address account, uint256 value) internal {\r\n        if (account == address(0)) {\r\n            revert ERC20InvalidReceiver(address(0));\r\n        }\r\n        minted += value;\r\n        _update(address(0), account, value);\r\n    }\r\n\r\n    /**\r\n     * @dev Destroys a `value` amount of tokens from `account`, lowering the total supply.\r\n     * Relies on the `_update` mechanism.\r\n     *\r\n     * Emits a {Transfer} event with `to` set to the zero address.\r\n     *\r\n     * NOTE: This function is not virtual, {_update} should be overridden instead\r\n     */\r\n    function _burn(address account, uint256 value) internal {\r\n        if (account == address(0)) {\r\n            revert ERC20InvalidSender(address(0));\r\n        }\r\n        burned += value;\r\n        _update(account, address(0), value);\r\n    }\r\n\r\n    /**\r\n     * @dev Sets `value` as the allowance of `spender` over the `owner` s tokens.\r\n     *\r\n     * This internal function is equivalent to `approve`, and can be used to\r\n     * e.g. set automatic allowances for certain subsystems, etc.\r\n     *\r\n     * Emits an {Approval} event.\r\n     *\r\n     * Requirements:\r\n     *\r\n     * - `owner` cannot be the zero address.\r\n     * - `spender` cannot be the zero address.\r\n     *\r\n     * Overrides to this logic should be done to the variant with an additional `bool emitEvent` argument.\r\n     */\r\n    function _approve(address owner, address spender, uint256 value) internal {\r\n        _approve(owner, spender, value, true);\r\n    }\r\n\r\n    /**\r\n     * @dev Variant of {_approve} with an optional flag to enable or disable the {Approval} event.\r\n     *\r\n     * By default (when calling {_approve}) the flag is set to true. On the other hand, approval changes made by\r\n     * `_spendAllowance` during the `transferFrom` operation set the flag to false. This saves gas by not emitting any\r\n     * `Approval` event during `transferFrom` operations.\r\n     *\r\n     * Anyone who wishes to continue emitting `Approval` events on the`transferFrom` operation can force the flag to\r\n     * true using the following override:\r\n     *\r\n     * ```solidity\r\n     * function _approve(address owner, address spender, uint256 value, bool) internal virtual override {\r\n     *     super._approve(owner, spender, value, true);\r\n     * }\r\n     * ```\r\n     *\r\n     * Requirements are the same as {_approve}.\r\n     */\r\n    function _approve(address owner, address spender, uint256 value, bool emitEvent) internal virtual {\r\n        if (owner == address(0)) {\r\n            revert ERC20InvalidApprover(address(0));\r\n        }\r\n        if (spender == address(0)) {\r\n            revert ERC20InvalidSpender(address(0));\r\n        }\r\n        _allowances[owner][spender] = value;\r\n        if (emitEvent) {\r\n            emit Approval(owner, spender, value);\r\n        }\r\n    }\r\n\r\n    /**\r\n     * @dev Updates `owner` s allowance for `spender` based on spent `value`.\r\n     *\r\n     * Does not update the allowance value in case of infinite allowance.\r\n     * Revert if not enough allowance is available.\r\n     *\r\n     * Does not emit an {Approval} event.\r\n     */\r\n    function _spendAllowance(address owner, address spender, uint256 value) internal virtual {\r\n        uint256 currentAllowance = allowance(owner, spender);\r\n        if (currentAllowance < type(uint256).max) {\r\n            if (currentAllowance < value) {\r\n                revert ERC20InsufficientAllowance(spender, currentAllowance, value);\r\n            }\r\n            unchecked {\r\n                _approve(owner, spender, currentAllowance - value, false);\r\n            }\r\n        }\r\n    }\r\n}\r\n";
        private string owner = "// SPDX-License-Identifier: MIT\r\npragma solidity ^0.8.0;\r\n\r\ncontract Owner{\r\n\r\n    address public owner;\r\n\r\n        constructor(address _owner)\r\n    {\r\n        owner = _owner;\r\n    }\r\n    \r\n    modifier onlyOwner() {\r\n        require(msg.sender == owner, \"Only the owner can perform this action.\");\r\n        _;\r\n    }\r\n}";
        private string lang;

        private bool isCode;


        private int maxDecimals = 3;
        private void LoadSettings()
        {
            string path = Directory.GetCurrentDirectory() + @"\settings.json";
          //  MessageBox.Show(path);
            try
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);

                    SettingsData settings = JsonSerializer.Deserialize<SettingsData>(jsonString);

                    if (settings != null)
                    {
                        lang = settings.Language;
                        string theme = settings.Theme;
                        Color hoverColor = settings.HoverColor;
                        bool closeOnStart = settings.CloseOnStart;


                        if(theme == "White")
                        {
                            Scheme_Ligher = Color.FromArgb(222, 222, 222);
                            Scheme_Darker = Color.Silver;
                            Scheme_Text = Color.FromArgb(64, 64, 64);
                        }
                        else if (theme == "Dark")
                        {
                            Scheme_Ligher = Color.DimGray;
                            Scheme_Darker = Color.FromArgb(75, 75, 75);
                            Scheme_Text = Color.LightGray;
                        }
                        else if (theme == "Black")
                        {
                            Scheme_Ligher = Color.FromArgb(44, 44, 44);
                            Scheme_Darker = Color.FromArgb(33, 33, 33);
                            Scheme_Text = Color.FromArgb(200, 200, 200);
                        }

                        // hlavni aplikace (form)
                        this.BackColor = Scheme_Darker;
                        pnl_top.BackColor = Scheme_Ligher;
                        pnl_advanced.BackColor = Scheme_Ligher;
                        pnl_simple.BackColor = Scheme_Ligher;
                        pnl_deluxe.BackColor = Scheme_Ligher;
                        pnl_choosecontract.BackColor = Scheme_Ligher;
                        pnl_choosecoin.BackColor = Scheme_Ligher;
                        tb_code.BackColor = Scheme_Ligher;
                        tb_code.ForeColor = Scheme_Text;
                        pnl_IDE_header.BackColor = Scheme_Ligher;
                        pnl_addVariable.BackColor = Scheme_Darker;
                        tb_console.BackColor = Scheme_Ligher;
                        tb_console.ForeColor = Scheme_Text;
                        pnl_tokenOtherSettings.BackColor = Scheme_Ligher;
                        pnl_tokensettings.BackColor = Scheme_Ligher;    
                        tb_tokenCode.BackColor = Scheme_Ligher;
                        tb_tokenCode.ForeColor = Scheme_Text;



                        DetectObjects(this);

                        if (closeOnStart)
                        {
                            MessageBox.Show("Close on start is enabled.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to deserialize settings.json.  Check the file format.");
                    }
                }
                else
                {
                    MessageBox.Show("settings.json file not found at: " + path + "Please make sure that you have succesfully installed Launcher, or the json file that contains settings does exists at the specified file path.");
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Error: Settings file not found. " + ex.Message + "Make sure you have succesfully installed Launcher, if not please install the Launcher.");
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Error: Directory not found for settings file. " + ex.Message);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Error: Could not parse settings.json. Invalid JSON format. " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
        }


        private void setCode()
        {

            string variableString = ""; 
            string variableFunction = "";
            string variableInnit = "";
            string variableRewrite = "";
            string variableView = "";
            string variableStruct = "";
            foreach (string variableBuilded in variablesBuilder)
            {
                variableString += variableBuilded + ", "; 
                variableFunction += " _" + variableBuilded + ",";
            }

            foreach (string _variableInnit in variablesInit)
            {
                variableInnit += _variableInnit + ", "; 
            }

            foreach (string _var in variables)
            {
                variableRewrite += "data." + _var + " = " + "_" + _var + ";\r\n        "; 
                variableView += _var + ": " + "_data." + _var + ",\r\n            ";
                variableStruct += _var + ": " + "_" + _var + ",\r\n            ";
                //variableFunction += " _" + _var + ",";
            }

            switch (contract_type)
            {
                case "simple":
                    Code = "module deployer::simple{\r\n  \r\n    use std::signer;\r\n    use std::vector;\r\n    use std::account;\r\n    use std::string;\r\n    use std::timestamp;\r\n    use std::table;\r\n    use std::debug::print;\r\n\r\n    const OWNER: address = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020;\r\n    \r\n    // ERROR CODES\r\n    const ERROR_NOT_OWNER: u64 = 1;\r\n\r\n    struct DATA has key, store, drop {" + variableString + "}\r\n\r\n\r\n\r\n\r\n    public entry fun storeDATA(address: &signer," + variableFunction + ") acquires DATA\r\n    {\r\n\r\n        let addr = signer::address_of(address);\r\n\r\n        if (!exists<DATA>(OWNER)) {\r\n            move_to(address, DATA {" + variableInnit + "});\r\n        };\r\n\r\n        //odesilatel musi byt owner\r\n        assert!(addr == OWNER, ERROR_NOT_OWNER);\r\n        let data = borrow_global_mut<DATA>(OWNER);\r\n\r\n        //prepsani starych hodnot/dat na nove\r\n        print(&number);\r\n        " + variableRewrite + "\r\n    }\r\n \r\n    #[view]\r\n    public fun viewDATA(): DATA acquires DATA\r\n    {\r\n        //\"pujceni\" ulozenych dat na adresse <OWNER>\r\n        let _data = borrow_global_mut<DATA>(OWNER);\r\n\r\n        //nacteni ulozenych dat do datoveho structu, ke kteremu patri\r\n        let data = DATA{\r\n            " + variableView + "\r\n        };\r\n\r\n        //debug\r\n        print(&data);\r\n        //return\r\n        move data\r\n    }\r\n \r\n\r\n \r\n    // Test function\r\n    #[test(account = @0x1, owner = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020)]\r\n    public entry fun test(account: signer, owner: signer) acquires DATA {\r\n        storeDATA(&owner, 5);\r\n        viewDATA();\r\n    }\r\n}";
                    break;
                case "advanced":
                    Code = "module deployer::advanced{\r\n  \r\n    use std::signer;\r\n    use std::vector;\r\n    use std::account;\r\n    use std::string;\r\n    use std::timestamp;\r\n    use std::table;\r\n    use std::debug::print;\r\n\r\n    const OWNER: address = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020;\r\n    \r\n    // ERROR CODES\r\n    const ERROR_NOT_OWNER: u64 = 1;\r\n\r\n    struct DATA has copy, key, store, drop {id: u64," + variableString + "}\r\n\r\n    struct HISTORICAL_DATA has key, store, drop, copy {database: vector<DATA>}\r\n\r\n    struct COUNTER has copy, key, store, drop {count: u64}\r\n\r\n\r\n    public entry fun storeDATA(address: &signer," + variableFunction + ") acquires DATA, HISTORICAL_DATA, COUNTER\r\n    {\r\n\r\n        let addr = signer::address_of(address);\r\n\r\n        if (!exists<DATA>(OWNER)) {\r\n            move_to(address, DATA {id: 0, " + variableInnit  + "});\r\n        };\r\n\r\n        if (!exists<HISTORICAL_DATA>(OWNER)) {\r\n            move_to(address, HISTORICAL_DATA { database: vector::empty() });\r\n        };\r\n\r\n        if (!exists<COUNTER>(OWNER)) {\r\n            move_to(address, COUNTER { count: 0 });\r\n        };\r\n\r\n        //odesilatel musi byt owner\r\n        assert!(addr == OWNER, ERROR_NOT_OWNER);\r\n        let data = borrow_global_mut<DATA>(OWNER);\r\n        let counter = borrow_global_mut<COUNTER>(OWNER);\r\n        //prepsani starych hodnot/dat na nove\r\n        let id_count = counter.count + 1;\r\n\r\n        let _data = DATA{\r\n            id: id_count,\r\n            " + variableStruct + "        };\r\n        print(&_data);\r\n        let database = borrow_global_mut<HISTORICAL_DATA>(OWNER);\r\n        vector::push_back(&mut database.database, _data);\r\n        counter.count = counter.count + 1;\r\n    }\r\n \r\n    #[view]\r\n    public fun viewDATA(count: u64): DATA acquires HISTORICAL_DATA\r\n    {\r\n        //\"pujceni\" ulozenych dat na adresse <OWNER>\r\n        assert!(exists<HISTORICAL_DATA>(OWNER), count);\r\n        let database = borrow_global<HISTORICAL_DATA>(OWNER);    \r\n        let _data = vector::borrow(&database.database, count);\r\n\r\n        //nacteni ulozenych dat do datoveho structu, ke kteremu patri\r\n        let data = DATA{\r\n            id: _data.id,\r\n            " + variableView + "        \r\n        };\r\n\r\n        //debug\r\n        print(&data);\r\n        //return\r\n        move data\r\n    }\r\n \r\n\r\n\r\n     #[view]\r\n    public fun viewALLDATA(): HISTORICAL_DATA acquires HISTORICAL_DATA\r\n    {\r\n        //\"pujceni\" ulozenych dat na adresse <OWNER>\r\n        let historical_data = *borrow_global<HISTORICAL_DATA>(OWNER);    \r\n        //let open_view = vector::borrow(&ohcl_Database.database, count);\r\n        //nacteni ulozenych dat do datoveho structu, ke kteremu patri\r\n        let _historical_data = HISTORICAL_DATA{\r\n            database: historical_data.database,\r\n        };\r\n\r\n        //debug\r\n        print(&_historical_data);\r\n        //return\r\n        move _historical_data\r\n    }\r\n\r\n \r\n    // Test function\r\n    #[test(account = @0x1, owner = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020)]\r\n    public entry fun test(account: signer, owner: signer) acquires DATA, HISTORICAL_DATA, COUNTER{\r\n        storeDATA(&owner, 5,2);\r\n        viewDATA(0);\r\n        storeDATA(&owner, 50,20);\r\n        viewALLDATA();\r\n    }\r\n}";
                    break;
                case "deluxe":
                    Code = "module deployer::deluxe{\r\n  \r\n    use std::signer;\r\n    use std::vector;\r\n    use std::account;\r\n    use std::timestamp;\r\n    use std::table;\r\n    use std::debug::print;\r\n    use std::string::utf8;\r\n    use std::string;\r\n\r\n    // TODO\r\n    // Pridat dalsi struct, ktery by ukladal stejne data jako DATA struct, akorat bez userID pro jednotlive uzivatele? Mozna by to davalo vetsi smysl pote pri vypisovani transakci uzivatele?\r\n    // Optimalizace + kontrola zabezpeceni\r\n\r\n    const OWNER: address = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020;\r\n    \r\n    // ERROR CODES\r\n    const ERROR_NOT_OWNER: u64 = 1;\r\n    const ERROR_VAR_NOT_INITIALIZED: u64 = 2;\r\n    const ERROR_TX_DOESNT_EXISTS: u64 = 3;\r\n    const ERROR_USER_DOESNT_EXISTS: u64 = 4;\r\n\r\n    struct DATA has copy, key, store, drop {userID: u64, txID: u128, timestamp: u64, " + variableString + "}\r\n\r\n    struct USER_DATA has copy,key,store,drop {id: u64, creation: u64, totalTX: u64 }\r\n\r\n    struct HISTORICAL_DATA has key, store, drop, copy {database: vector<DATA>}\r\n\r\n    struct TOTAL has key, store, drop, copy {totalUsers: u64, totalTX: u128}\r\n\r\n    struct USER_TRANSACTIONS_DATABASE has key { transactions: table::Table<u64, vector<DATA>> }\r\n\r\n    struct USER_DATABASE has key { users: table::Table<u64, USER_DATA> }\r\n    \r\n    struct DATABASE has copy, key, drop { database: vector<DATA> }\r\n\r\n\r\n    entry fun innit(address: &signer) {\r\n        //The transaction sender needs to be OWNER, otherwise returns error with a predefined code 1.\r\n        let addr = signer::address_of(address);\r\n        assert!(addr == OWNER, ERROR_NOT_OWNER);\r\n\r\n        let user_data = USER_DATA{\r\n            id: 0,\r\n            creation: 0,\r\n            totalTX: 0,\r\n        };\r\n\r\n        if (!exists<DATA>(OWNER)) {\r\n            move_to(address, DATA {userID: 0, txID: 0,timestamp: 0," + variableInnit + "});\r\n        };\r\n\r\n        if (!exists<HISTORICAL_DATA>(OWNER)) {\r\n            move_to(address, HISTORICAL_DATA { database: vector::empty() });\r\n        };\r\n\r\n        if (!exists<TOTAL>(OWNER)) {\r\n            move_to(address, TOTAL { totalUsers: 0, totalTX: 0 });\r\n        };\r\n\r\n\r\n        if (!exists<USER_TRANSACTIONS_DATABASE>(OWNER)) {\r\n            move_to(address, USER_TRANSACTIONS_DATABASE { transactions: table::new<u64, vector<DATA>>() });\r\n        };\r\n\r\n        if (!exists<USER_DATABASE>(OWNER)) {\r\n            let users_table = table::new<u64, USER_DATA>();\r\n            move_to(address, USER_DATABASE { users: users_table });\r\n        };\r\n    }\r\n\r\n    public entry fun storeDATA(address: &signer, _userID: u64," + variableFunction + ") acquires DATA, HISTORICAL_DATA, USER_DATABASE, USER_TRANSACTIONS_DATABASE, TOTAL\r\n    {\r\n\r\n        let addr = signer::address_of(address);\r\n        //The transaction sender needs to be OWNER, otherwise returns error with a predefined code 1.\r\n        assert!(addr == OWNER, ERROR_NOT_OWNER);\r\n\r\n        innit(address);\r\n\r\n        let data = borrow_global_mut<DATA>(OWNER);\r\n        let database_table = borrow_global_mut<USER_TRANSACTIONS_DATABASE>(OWNER);\r\n        let users_table = borrow_global_mut<USER_DATABASE>(OWNER);\r\n        let total_stats = borrow_global_mut<TOTAL>(OWNER);\r\n\r\n        //Getting current unix time epoch from blockchain. (calling the now_seconds functions which returns unix epoch in seconds, replacable with now_microseconds()).\r\n        let time = timestamp::now_seconds();\r\n\r\n        if (table::contains(&users_table.users, _userID)) {\r\n            let user = table::borrow_mut(&mut users_table.users, _userID);\r\n            user.totalTX = user.totalTX + 1;\r\n        } else {\r\n            let user_data = USER_DATA{\r\n                id: _userID,\r\n                creation: time,\r\n                totalTX: 1,\r\n            };\r\n            table::add(&mut users_table.users, _userID, user_data);\r\n            total_stats.totalUsers = total_stats.totalUsers + 1;\r\n        };\r\n\r\n        let userCount = total_stats.totalUsers;\r\n        assert!(_userID <= userCount, ERROR_USER_DOESNT_EXISTS);\r\n\r\n        let tx_id = total_stats.totalTX;\r\n        let _data = DATA{\r\n            userID: _userID,\r\n            txID: tx_id,\r\n            timestamp: time,\r\n            " + variableStruct + "\r\n        };\r\n\r\n        // Add the transaction to the user's transaction list\r\n        if (table::contains(&database_table.transactions, _userID)) {\r\n            let transactions = table::borrow_mut(&mut database_table.transactions, _userID);\r\n            vector::push_back(transactions, _data);\r\n        } else {\r\n            let transactions = vector::empty<DATA>();\r\n            vector::push_back(&mut transactions, _data);\r\n            table::add(&mut database_table.transactions, _userID, transactions);\r\n        };\r\n\r\n        print(&_data);\r\n        let database = borrow_global_mut<HISTORICAL_DATA>(OWNER);\r\n        vector::push_back(&mut database.database, _data);\r\n\r\n        total_stats.totalTX = total_stats.totalTX + 1;\r\n\r\n    }\r\n \r\n    #[view]\r\n    public fun viewDATA(count: u64): DATA acquires HISTORICAL_DATA\r\n    {\r\n        //\"pujceni\" ulozenych dat na adresse <OWNER>\r\n        assert!(exists<HISTORICAL_DATA>(OWNER), count);\r\n        let database = borrow_global<HISTORICAL_DATA>(OWNER);    \r\n        let _data = vector::borrow(&database.database, count);\r\n\r\n        //nacteni ulozenych dat do datoveho structu, ke kteremu patri\r\n        let data = DATA{\r\n            userID: _data.userID,\r\n            txID: _data.txID,\r\n            timestamp: _data.timestamp,\r\n            " + variableView +"\r\n        };\r\n\r\n        //debug\r\n        print(&data);\r\n        //return\r\n        move data\r\n    }\r\n\r\n    #[view]\r\n    public fun view_USER_TRANSACTIONS(userID: u64): vector<DATA> acquires USER_TRANSACTIONS_DATABASE {\r\n        let tx_database = borrow_global<USER_TRANSACTIONS_DATABASE>(OWNER);\r\n        let transactions = *table::borrow(&tx_database.transactions, userID);\r\n        move transactions\r\n    }\r\n\r\n\r\n    #[view]\r\n    public fun viewTotalDATA(): TOTAL acquires TOTAL {\r\n        let total_stats = borrow_global<TOTAL>(OWNER);\r\n\r\n        let _total_stats = TOTAL{\r\n            totalTX: total_stats.totalTX,\r\n            totalUsers: total_stats.totalUsers,\r\n        };\r\n        move _total_stats\r\n    }\r\n\r\n     #[view]\r\n    public fun viewALLDATA(): HISTORICAL_DATA acquires HISTORICAL_DATA\r\n    {\r\n        //\"pujceni\" ulozenych dat na adresse <OWNER>\r\n        let historical_data = *borrow_global<HISTORICAL_DATA>(OWNER);    \r\n        //let open_view = vector::borrow(&ohcl_Database.database, count);\r\n        //nacteni ulozenych dat do datoveho structu, ke kteremu patri\r\n        let _historical_data = HISTORICAL_DATA{\r\n            database: historical_data.database,\r\n        };\r\n\r\n        //debug\r\n        print(&_historical_data);\r\n        //return\r\n        move _historical_data\r\n    }\r\n\r\n\r\n    #[view]\r\n    public fun view_USER_STATS(userID: u64): USER_DATA acquires USER_DATABASE {\r\n        let userTable = borrow_global<USER_DATABASE>(OWNER);\r\n        assert!(table::contains(&userTable.users, userID), ERROR_VAR_NOT_INITIALIZED);\r\n        let user_data = *table::borrow(&userTable.users, userID);\r\n\r\n        let _user_data = USER_DATA{\r\n            id: user_data.id,\r\n            creation: user_data.creation,\r\n            totalTX: user_data.totalTX,\r\n        };\r\n        print(&_user_data);\r\n        move _user_data\r\n    }\r\n\r\n \r\n    // Test function\r\n    #[test(account = @0x1, owner = @0xc698c251041b826f1d3d4ea664a70674758e78918938d1b3b237418ff17b4020)]\r\n    public entry fun test(account: signer, owner: signer) acquires DATA, HISTORICAL_DATA, USER_TRANSACTIONS_DATABASE, USER_DATABASE, TOTAL{\r\n        timestamp::set_time_has_started_for_testing(&account);  \r\n        print(&utf8(b\" Executing storeDATA...\"));\r\n        storeDATA(&owner, 0 ,5,2);\r\n        print(&utf8(b\" Executing storeDATA...\"));\r\n        storeDATA(&owner, 0 ,5,2);\r\n        print(&utf8(b\" Executing storeDATA...\"));\r\n        storeDATA(&owner, 0 ,5,2);\r\n        print(&utf8(b\"First transaction... (viewDATA)\"));\r\n        viewDATA(0);\r\n        print(&utf8(b\" Executing storeDATA...\"));\r\n        storeDATA(&owner, 1,50,20);\r\n        print(&utf8(b\"View of all stored DATA... (viewALLDATA)\"));\r\n        viewALLDATA();\r\n        print(&utf8(b\"Data of user with ID 0... (view_USER_STATS)\"));\r\n        view_USER_STATS(0);\r\n        print(&utf8(b\"Total Stats... (viewTotalDATA)\"));\r\n        let viewTotalDATA = viewTotalDATA();\r\n        print(&viewTotalDATA);\r\n        print(&utf8(b\" All stored data of user with ID 1 ... (view_USER_TRANSACTIONS)\"));\r\n        let viewUserTX = view_USER_TRANSACTIONS(0);\r\n        print(&viewUserTX);\r\n    }\r\n}";
                    break;
            }
            tb_code.Text = Code;
        }

        private void DetectObjects(Control parent) // zdroj: https://learn.microsoft.com/en-us/answers/questions/1380697/controls-inside-controls
        {
            foreach (Control control in parent.Controls)
            {
                // detekce vsech buttonu, labelu a checkboxu
                if (control.Name == "label9" || control.Name == "label11" || control.Name == "label10")
                {
                    control.ForeColor = Color.FromArgb(50, 235, 230);
                }
                else if (control.Name == "label8") // Check for label9 FIRST
                {
                    control.ForeColor = Color.FromArgb(100, 255, 255);
                }
                else if (control.Name == "label7") // Check for label9 FIRST
                {
                    control.ForeColor = Color.FromArgb(150, 255, 255);
                }
                else if (control is Button btn || control is Label lbl || control is CheckBox checkB) 
                {
                    control.ForeColor = Scheme_Text;
                }
                // detekce pokud je objekt napriklad vlozen v panelu

                else if (control.HasChildren)
                {
                    DetectObjects(control);
                }
            }
        }

        private void language()
        {
            string languageCode = "";

            switch (lang)
            {
                case "Čeština (Czech)":
                    languageCode = "cs-CZ";
                    break;
                case "English":
                    languageCode = "";
                    break;
                case "中國人 (Chinese)":
                    languageCode = "zh-CN";
                    break;
                case "Tiếng Việt (Vietnamese)":
                    languageCode = "vi-VN";
                    break;
                case "한국어 (Korean)":
                    languageCode = "ko-KR";
                    break;
            }

            LanguageHandler(languageCode);

            //   lbl_Exchange.Text = trading_simulator.Properties.Resources.Exchange;
            // about_dca.Text = trading_simulator.Properties.Resources.dca_info;
            lbl_data_storageInfo.Text = Properties.Resources.dataStorageInfo;
            btn_memecoin.Text = Properties.Resources._continue;
            btn_smartcontract.Text = Properties.Resources._continue;
            lbl_tokenSymbol.Text = Properties.Resources.tokenSymbol;
            lbl_tokenDecimals.Text = Properties.Resources.tokenDecimals;
            lbl_tokenSupply.Text = Properties.Resources.tokenSupply;
            lbl_tokenName.Text = Properties.Resources.tokenName;
            btn_generateToken.Text = Properties.Resources.tokenGenerate;
            lbl_Fees.Text = Properties.Resources.tokenFee;
            lbl_feeInfo.Text = Properties.Resources.tokenfeeInfo;
            lbl_burnFee.Text = Properties.Resources.tokenburnFee;
            lbl_txfee.Text = Properties.Resources.tokentxFee;
            cb_Mintable.Text = Properties.Resources.tokenMintable;
            cb_Burnable.Text = Properties.Resources.tokenBurnable;
            lbl_staking.Text = Properties.Resources.tokenStaking;
            lbl_stakingInfo.Text = Properties.Resources.tokenstakingInfoa;
            lbl_Simple.Text = Properties.Resources.simpleInfo;
            lbl_Advanced.Text = Properties.Resources.advancedInfo;
            lbl_Deluxe.Text = Properties.Resources.deluxeInfo;
            btn_addVariable.Text = Properties.Resources.addVariable;
            btn_deleteVariable.Text = Properties.Resources.deleteVariable;
            btn_initializeVariable.Text = Properties.Resources.addVariable;
            lbl_variableName.Text = Properties.Resources.variableName;
            lbl_variableType.Text = Properties.Resources.variableType;
            lbl_ownerPrivileges.Text = Properties.Resources.tokenPrivileges;
            btn_deluxe.Text = Properties.Resources.String1;
            lbl_cryptoInfo.Text = Properties.Resources.cryptoInfo;
            btn_simple.Text = Properties.Resources.String1;
            btn_advanced.Text = Properties.Resources.String1;
            lbl_FeeCollector.Text = Properties.Resources.feeCollector;
            btn_viewVariables.Text = Properties.Resources.viewVariables;
            btn_Package.Text = Properties.Resources.Package;
            btn_code.Text = Properties.Resources.Code_;
            btn_initializeVariable.Text = Properties.Resources.addVariable;
        }

        private void LanguageHandler(string language) // zdroj: https://stackoverflow.com/questions/32989100/how-to-make-multi-language-app-in-winforms
        {

            //https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread.currentculture?view=net-9.0 mozna optimalizace, udelat vlastni ddl rozsireni?
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
            language();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
           // MovableForm.MovableForm.MoveForm(e, MouseButtons.Left, Handle);
        }

        private void btn_close_Click(object sender, EventArgs e)
        {

            // Původní "nefunkční" řešení
            // Application.Exit();

            // Fungující řešení, které detekuje winform aplikace podle názvu.
            // Zdroj: https://learn.microsoft.com/en-us/answers/questions/634918/show-a-form-if-already-is-running-c
            var thisForm = Application.OpenForms.Cast<Form>()
                               .FirstOrDefault(f => f.Name == "ContractGenerator");
            thisForm.Close();
            Class2.launched = false;
        }

        private void btn_minimalize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void btn_smartcontract_Click(object sender, EventArgs e)
        {
            btn_back.Visible = true;
            pnl_welcome.Visible = false;
            pnl_smartcontracts.Visible = true;
            checkGoBackButton("Contracts");

        }

        private void btn_simple_Click(object sender, EventArgs e)
        {
            checkGoBackButton("IDE");
            contract_type = "simple";
            setCode();
            pnl_IDE.Visible = true;
            pnl_smartcontracts.Visible = false;
        }

        private void btn_closeAddVariable_Click(object sender, EventArgs e)
        {
            pnl_addVariable.Visible = false;
        }

        private void btn_addVariable_Click(object sender, EventArgs e)
        {
            pnl_addVariable.Visible=true;
            tb_addVariable.Text = "";
            cb_addVariable.SelectedIndex = -1;
            btn_initializeVariable.Text = "Add";
        }


        private void btn_deleteVariable_Click(object sender, EventArgs e)
        {
            pnl_addVariable.Visible = true;
            tb_addVariable.Text = "";
            cb_addVariable.SelectedIndex = -1;
            btn_initializeVariable.Text = "Delete";
        }

        private void varBuilder(bool add, string _varName, string _varType)
        {
            switch(_varType)
            {
                case "u8":
                    var = _varName + ": " + 0;
                    break;
                case "u16":
                    var = _varName + ": " + 0;
                    break;
                case "u32":
                    var = _varName + ": " + 0;
                    break;
                case "u64":
                    var = _varName + ": " + 0;
                    break;
                case "u128":
                    var = _varName + ": " + 0;
                    break;
                case "u256":
                    var = _varName + ": " + 0;
                    break;
                case "bool":
                    var = _varName + ": " + "false";
                    break;
                case "vector<u8>":
                    var = _varName + ": " + "vector::empty()";
                    break;
                case "address":
                    var = _varName + ": " + "@0x1";
                    break;
            }
            string varBuilder;
            if(_varType == "vector<u8>")
            {
                varBuilder = _varName + ": " + "vector<u8>";
            }
            else
            {
                varBuilder = _varName + ": " + _varType;
            }

            if (add == true)
            {
                variablesInit.Add(var);
                variablesBuilder.Add(varBuilder);
                variables.Add(_varName);
            }
            else if (add == false) {
                variablesInit.Remove(var);
                variablesBuilder.Remove(varBuilder);
                variables.Remove(_varName);
            }
        }



        private void Variable(bool add, string name, string type)
        {

            string _varBuilder = name + ": " + type;

            if (add == false)
            {
                MessageBox.Show(variables.Contains(name).ToString());
                if (variables.Contains(name))
                {
                    if(variablesBuilder.Contains(_varBuilder))
                    {
                        varBuilder(false, name, type);
                        console(false, $"Variable {name} succesfully deleted.");
                    }
                    else
                    {
                        console(true, $"Error Variable {name} does exists, but the type is wrong.");
                    }

                }
                else
                {
                    console(true, "Variable does not exists.");
                }
            }
            else if (add == true)
            {
                varBuilder(true, name, type);
                string outputText = "Stored Builded Variables:\n";
                foreach (string variable in variablesBuilder)
                {
                    outputText += variable + "\n";
                }
                outputText += "Stored Variables:\n";
               foreach (string variableName in variables)
               {   
                    outputText += variableName + "\n";
               }

                console(false, "Success! Variable:" + name + " with type: " + type + " has been succesfully added.");
            }
            setCode();
            pnl_addVariable.Visible = false;
        }

        private void btn_initializeVariable_Click(object sender, EventArgs e)
        {
           if (tb_addVariable.Text == string.Empty || cb_addVariable.Text == string.Empty)
            {
                console(true, "Please, make sure that you filled the name of your variable, and choosed supported type.");
            }
           else
            {
                if (btn_initializeVariable.Text == "Delete")
                {
                    Variable(false, tb_addVariable.Text, cb_addVariable.Text);
                }
                else if (btn_initializeVariable.Text == "Add")
                {
                    Variable(true, tb_addVariable.Text, cb_addVariable.Text);
                }
            }
            MessageBox.Show(tb_addVariable.Text, cb_addVariable.Text);
        }



        private void btn_advanced_Click(object sender, EventArgs e)
        {
            checkGoBackButton("IDE");
            contract_type = "advanced";
            setCode();
            pnl_IDE.Visible = true;
            pnl_smartcontracts.Visible = false;
        }

        private void btn_deluxe_Click(object sender, EventArgs e)
        {
            checkGoBackButton("IDE");
            contract_type = "deluxe";
            setCode();
            pnl_IDE.Visible = true;
            pnl_smartcontracts.Visible = false;
        }
        private void btn_copy_Click_1(object sender, EventArgs e)
        {
            if(Code == null || Code == string.Empty)
            {
                console(true, "Did not copy anything, because string Code is empty.");
            }
            else
            {
                // zdroj: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.clipboard.settext?view=windowsdesktop-9.0
                Clipboard.SetText(Code);
                console(false, "Succesfully copied code to clipboard");
            }
        }

        private void console(bool isError, string msg)
        {
            DateTime dateTime = DateTime.Now;
            dateTime.ToUniversalTime();

            string _bool;

            if(isError == false)
            {
                _bool = "SUCCESS";
            }
            else
            {
                _bool = "ERROR";
            }

            tb_console.Text += "\n" + dateTime + " | " + _bool + " | " + msg + "\r\n";
        }

        private void btn_save_Click(object sender, EventArgs e)
        {

            string type;
            // zdroj: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-9.0
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            if (isCode == true)
            {
                saveFileDialog1.Filter = "Move (*.move)|*.move|All files (*.*)|*.*";
                type = "module";
            }
            else
            {
                saveFileDialog1.Filter = "Toml (*.toml)|*.toml|All files (*.*)|*.*";
                type = "package";
            }
            saveFileDialog1.AddExtension = true;

            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
               
                string filePath = saveFileDialog1.FileName;

                File.WriteAllText(filePath, Code);
                console(false, "Succesfully saved your " + type + " in file path: " + filePath);
            }
            else
            {
                console(true, "Saving was cancelled.");
            }
        }

        private void btn_generateToken_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_Name.Text))
            {
                tb_console.Text += _console.output(true, "Please enter the name of your token");
                return; 
            }

            if (string.IsNullOrEmpty(tb_Symbol.Text))
            {
                tb_console.Text += _console.output(true, "Please enter the symbol of your token");
                return; 
            }

            if (tb_Decimals.Value == 0)
            {
                tb_console.Text += _console.output(true, "Please enter valid decimals (min. 1)");
                return; 
            }

            if (tb_Supply.Value == 0)
            {
                tb_console.Text += _console.output(true, "Please enter valid supply (min. 1 token)");
                return;
            }

            if (tb_Fee.Value != 0 || tb_BurnFee.Value != 0 )
            {
                string input = tb_FeeCollector.Text;
                if (string.IsNullOrEmpty(input) || !input.StartsWith("0x"))
                {
                    tb_console.Text += _console.output(true, "Please enter valid fee Collector adress.");
                    return;
                }
              
            }


            name = tb_Name.Text;
            symbol = tb_Symbol.Text;
            decimals = (int)tb_Decimals.Value;
            supply = (float)tb_Supply.Value;
            _supply = supply.ToString("0");
            burnfee = (float)tb_BurnFee.Value;
            transferFee = (float)tb_Fee.Value;
            fee = (float)tb_Fee.Value;
            burnable = cb_Burnable.Checked;
            mintable = cb_Mintable.Checked;
            stakingAPR = (float)tb_APR.Value;
            feeCollector = tb_FeeCollector.Text;

            tb_console.Text += _console.output(false, "\n" +
                "Token name: " + name + "\n" +
                "Token symbol: " + symbol + "\n" +
                "Decimals: " + decimals + "\n" +
                "Supply: " + supply.ToString("0") + "\n" +
                "Burn fee: " + burnfee + " %" + "\n" +
                "Transfer fee: " + transferFee +  " %" +  "\n" +
                "Fee: " + fee + " %" + "\n" + 
                "Mintable: " + mintable + "\n" +
                "Burnable: " + burnable + "\n" +
                "Staking APR: " + stakingAPR + " %"

                );
          //  pnl_tokenCode.Visible = true;
            //pnl_token.Visible = false;
            tb_tokenCode.Text = generateCode();
            checkGoBackButton("TokenCode");
        }


        private string generateCode()
        {

            float _burnfee = burnfee * (float)Math.Pow(10, maxDecimals);
            float _transferFee = transferFee * (float)Math.Pow(10, maxDecimals);

            string _maxDecimals = $"uint8 maxDecimals = {maxDecimals};";
            string _burnFee_ = (burnfee == 0) ? $"uint8 public BurnFee = 0;" : $"uint32 public BurnFee = {_burnfee};";

            string _transferFee_ = (transferFee == 0) ? $"uint8 public TransferFee = 0;" : $"uint32 public TransferFee = {_transferFee};";
            string _feeCollector = (burnfee == 0) || (transferFee == 0) ? string.Empty : $"address public FeeCollector = {feeCollector};";
            string _burnable = (burnable == false) ? string.Empty : 
                "" +
                "    function burn(uint256 amount) public onlyOwner {\r\n" +
                "       require(balanceOf(msg.sender) >= amount, \"Not enough tokens to burn.\");" +
                "\r\n   _burn(msg.sender, amount);" +
                "\r\n    }";

            string _mintable = (mintable == false) ? string.Empty : 
                "   " +
                "   function mint(uint256 amount) public onlyOwner {\r\n" +
                "       _mint(msg.sender, amount);\r\n" +
                "    }";

            string _staking = (stakingAPR == 0) ? string.Empty : 
                "\r\n\r\n    struct _Stake {" +
                "\r\n        uint256 amount;" +
                "\r\n        uint256 time;" +
                "\r\n    " +
                "}" +
                "\r\n\r\n " +
                "   mapping(address => _Stake) public AddressStaked;\r\n\r\n" +
                $"    uint8 public StakingAPR = {stakingAPR};\r\n\r\n" +
                "    function stake(uint256 amount) public returns (bool) {" +
                "\r\n        require(balanceOf(msg.sender) >= amount, \"Not enough tokens to stake.\");" +
                "\r\n         AddressStaked[msg.sender] = _Stake(amount, block.timestamp);" +
                "\r\n         _transfer(msg.sender, address(this), amount);" +
                "\r\n        return true;\r\n    }" +
                "\r\n\r\n    // Zmenit v budoucnu z rocni odmeny na treba tydenni, nebo udelat nejakej algoritmus ze by to treba slo i po hodinne nebo realtime? Jestli to je vubec mozne." +
                "\r\n    function withdraw(uint256 amount) public returns (bool){" +
                "\r\n         _Stake storage stake_ = AddressStaked[msg.sender]; " +
                "\r\n        require(stake_.amount  > 0, \"Not staked any tokens.\");" +
                "\r\n        require(stake_.amount  >= amount, \"Cannot withdraw more tokens than staked!\");" +
                "\r\n        _transfer(address(this), msg.sender, stake_.amount);" +
                "\r\n        uint256 reward = amount * StakingAPR * (block.timestamp - stake_.time) / 3155692;" +
                "\r\n        AddressStaked[msg.sender] = _Stake(0, block.timestamp);" +
                "\r\n        _mint(msg.sender, reward);" +
                "\r\n        return true;  " +
                "\r\n    }";
            string _fee =
                $"  {_maxDecimals}" +
                "\r\n" +
                $"  {_transferFee_}" +
                "\r\n" +
                $"   {_burnFee_}" +
                "\r\n" +
                $"   {_feeCollector}" +
                "\r\n" +
                "   function transfer(address to, uint256 amount) public override returns (bool) {" +
                "\r\n        require(balanceOf(msg.sender) >= amount, \"Not enough tokens to send.\");" +
                "\r\n        // 100 * 1) / 100 )" +
                "\r\n        uint256 burnTax = ((amount * BurnFee) / (10 ** maxDecimals)) / 100;" +
                "\r\n        uint256 transferTax = ((amount * TransferFee) / (10 ** maxDecimals)) /100;" +
                "\r\n        uint256 _amount = amount - (burnTax + transferTax);" +
                "\r\n        _transfer(msg.sender, to, _amount);" +
                "\r\n        _transfer(msg.sender, FeeCollector, transferTax);" +
                "\r\n        _burn(msg.sender, burnTax);" +
                "\r\n        return true;" +
                "\r\n    }";
            
            string code = $"// SPDX-License-Identifier: MIT\r\n" +
                "// Compatible with OpenZeppelin Contracts ^4.0.0\r\n" +
                "pragma solidity ^0.8.22;\r\n\r\n" +
                "import {ERC20} from \"@openzeppelin/contracts/token/ERC20/ERC20.sol\";" +
                "\r\n//import {ERC20Burnable} from \"@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol\";" +
                "\r\nimport {ERC20Permit} from \"@openzeppelin/contracts/token/ERC20/extensions/ERC20Permit.sol\";" +
                "\r\nimport {Owner} from \"./Owner.sol\";\r\n\r\ncontract Aexis is ERC20, ERC20Permit, Owner " +
                "{\r\n\r\n " +
                $"  \r\nuint256 supply = {_supply};" +
                $"  \r\nuint8 _Decimals = {decimals};" +
                "\r\n\r\n    constructor()" +
                "\r\n        ERC20(\"Aexis Test\", \"AXS\", _Decimals)" +
                "\r\n        ERC20Permit(\"Aexis Test\")" +
                "\r\n        Owner(msg.sender)\r\n    {" +
                "\r\n        _mint(msg.sender, supply);" +
                "\r\n    }" +
                 $"{_mintable}" +
                "\r\n    " +
                "\r\n\r\n " +
                 $"{_burnable}" +
                "\r\n\r\n " +
                 $"{_fee}" +
                "\r\n\r\n " +
                 $"{_staking}" +
                "\r\n}";
            return code;
        }


        private void btn_memecoin_Click(object sender, EventArgs e)
        {
            checkGoBackButton("Token");
           // MessageBox.Show(back.ToString(), back.ToString());
            btn_back.Visible = true;
        //    pnl_token.Visible = true;
      //      pnl_welcome.Visible = false;
       //     tb_console.Visible = true;
        }

        private void btn_back_Click(object sender, EventArgs e)
        {

            if (back == "Token")
            {
                checkGoBackButton("Welcome");
            }
            if (back == "IDE")
            {
                checkGoBackButton("Contracts");
            }
            if (back == "Contracts")
            {
                checkGoBackButton("Welcome");
            }
            if(back == "TokenCode")
            {
                checkGoBackButton("Token");
            }

        }

        private void pnl_welcome_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnl_choosecoin_Paint(object sender, PaintEventArgs e)
        {

        }


        private void checkGoBackButton(string code)
        {
            back = code;
            pnl_welcome.Visible = false;
            pnl_IDE.Visible = false;
            pnl_smartcontracts.Visible = false;
            pnl_token.Visible = false;
            pnl_tokenCode.Visible = false;
            
            switch (back)
            {
                case "Welcome":
                    pnl_welcome.Visible=true;
                    return;
                case "Contracts":
                    pnl_smartcontracts.Visible = true;
                    return;
                case "IDE":
                    MessageBox.Show("Continue by adding your first desired variable", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    pnl_IDE.Visible = true;
                    return;
                case "Token":
                    pnl_token.Visible = true;
                    return;
                case "TokenCode":
                    pnl_tokenCode.Visible = true;
                    return;
            }
        }

        private void btn_viewVariables_Click(object sender, EventArgs e)
        {

            StringBuilder allvariables = new StringBuilder();

            foreach (string _variableBuilder in variablesBuilder)
            {
                allvariables.Append("\n" + _variableBuilder);
                console(false, allvariables.ToString());
            }
        }

        private void pnl_top_MouseDown(object sender, MouseEventArgs e)
        {
            MovableForm.MovableForm.MouseDown(e, MouseButtons.Left, Handle);
        }

        private void btn_Package_Click(object sender, EventArgs e)
        {
            isCode = false;
            string module_type = contract_type.ToString();
            string package = "[package]" +
                "\r\nname = \"" + module_type + "\"" +
                "\r\nversion = \"1.0.0\"" +
                "\r\nauthors = []" +
                "\r\n\r\n[addresses]" +
                "\r\ndeployer = \"your_adress\"" +
                "\r\n[dev-addresses]\r\n" +
                "\r\n[dependencies.SupraFramework]" +
                "\r\ngit = \"https://github.com/Entropy-Foundation/aptos-core.git" +
                "\"\r\nrev = \"dev" +
                "\"\r\nsubdir = \"aptos-move/framework/supra-framework\"\r\n" +
                "\r\n[dev-dependencies]";
            tb_code.Text = package;
        }

        private void btn_code_Click(object sender, EventArgs e)
        {
            setCode();
            isCode = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tb_tokenCode.Text = owner;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            tb_tokenCode.Text = erc20;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tb_tokenCode.Text = generateCode();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (tb_tokenCode.Text == null || tb_tokenCode.Text == string.Empty)
            {
                console(true, "Did not copy anything, because tb_tokenCode.Text is empty.");
            }
            else
            {
                // zdroj: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.clipboard.settext?view=windowsdesktop-9.0
                Clipboard.SetText(tb_tokenCode.Text);
                console(false, "Succesfully copied code to clipboard");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string type;
            // zdroj: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=windowsdesktop-9.0
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.RestoreDirectory = true;
            if (isCode == true)
            {
                saveFileDialog1.Filter = "Solidity (*.Sol)|*.Sol|All files (*.*)|*.*";
                type = "module";
            }
            else
            {
                saveFileDialog1.Filter = "Solidity (*.Sol)|*.Sol|All files (*.*)|*.*";
                type = "package";
            }
            saveFileDialog1.AddExtension = true;

            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {

                string filePath = saveFileDialog1.FileName;

                File.WriteAllText(filePath, Code);
                console(false, "Succesfully saved your " + type + " in file path: " + filePath);
            }
            else
            {
                console(true, "Saving was cancelled.");
            }
        }
    }
}
