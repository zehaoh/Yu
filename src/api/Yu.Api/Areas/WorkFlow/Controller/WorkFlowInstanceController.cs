
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities;
using Yu.Model.Common.InputModels;
using Yu.Data.Entities.WorkFlow;
using Yu.Service.WorkFlow.WorkFlowInstances;
using Yu.Core.Extensions;
using System.Collections.Generic;
using Yu.Model.WorkFlow.WorkFlowInstance.InputModels;
using System;
using Yu.Model.Message;

namespace Yu.Api.Areas.WorkFlow.Controller
{
    [Route("api")]
    [Description("工作流实例")]
    public class WorkFlowInstanceController : AuthorizeController
    {
        private readonly IWorkFlowInstanceService _service;

        public WorkFlowInstanceController(IWorkFlowInstanceService service)
        {
            _service = service;
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("workflowInstance")]
        [Description("取得工作流实例数据")]
        public IActionResult GetWorkFlowInstances([FromQuery] PagedQuery query)
        {
            var result = _service.GetWorkFlowInstances(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        [HttpPost("workflowInstance")]
        [Description("添加工作流实例数据")]
        public async Task<IActionResult> AddWorkFlowInstance([FromBody]WorkFlowInstance entity)
        {
            entity.UserName = User.GetUserName();
            await _service.AddWorkFlowInstanceAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 取得工作流实例表单数据
        /// </summary>
        [HttpGet("workflowInstanceForm")]
        [Description("取得工作流实例表单数据")]
        public IActionResult GetWorkFlowInstanceForm([FromQuery]Guid id)
        {
            var forms = _service.GetWorkFlowInstanceForm(id);
            return Ok(forms);
        }

        /// <summary>
        /// 更新工作流实例表单数据
        /// </summary>
        [HttpPut("workflowInstanceForm")]
        [Description("更新工作流实例表单数据")]
        public async Task<IActionResult> AddOrUpdateWorkFlowInstanceForm([FromBody]WorkFlowInstanceFormInputViewModel model)
        {
            await _service.AddOrUpdateWorkFlowInstanceForm(model.InstanceId, model.WorkFlowInstanceForms);
            return Ok();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        [HttpPut("workflowInstance")]
        [Description("更新工作流实例数据")]
        public async Task<IActionResult> UpdateWorkFlowInstance([FromBody]WorkFlowInstance entity)
        {
            await _service.UpdateWorkFlowInstanceAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        [HttpDelete("workflowInstance")]
        [Description("删除工作流实例数据")]
        public async Task<IActionResult> DeleteWorkFlowInstance([FromQuery]IdQuery query)
        {
            await _service.DeleteWorkFlowInstanceAsync(query.Id);
            return Ok();
        }

        #region 逻辑删除工作流实例

        /// <summary>
        /// 取得回收站工作流实例数据
        /// </summary>
        [HttpGet("deltetedWorkflowInstance")]
        [Description("取得回收站工作流实例数据")]
        public IActionResult GetDeletedWorkFlowInstanceForm([FromQuery] PagedQuery query)
        {
            var forms = _service.GetDeletedWorkFlowInstanceForm(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(forms);
        }

        /// <summary>
        /// 工作流实例加入回收站
        /// </summary>
        [HttpPut("deltetedWorkflowInstance")]
        [Description("工作流实例加入回收站")]
        public async Task<IActionResult> LogicDeletedWorkFlowInstance([FromBody]IdQuery query)
        {
            var success = await _service.SetWorkFlowInstanceDelete(query.Id, true);
            if (!success)
            {
                ModelState.AddModelError("IsDelete", ErrorMessages.WorkFlow_Instance_E001);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        /// <summary>
        /// 工作流实例加入回收站
        /// </summary>
        [HttpPatch("deltetedWorkflowInstance")]
        [Description("工作流实例从回收站取回")]
        public async Task<IActionResult> LogicRebackWorkFlowInstance([FromBody]IdQuery query)
        {
            await _service.SetWorkFlowInstanceDelete(query.Id, false);
            return Ok();
        }

        #endregion
    }
}

