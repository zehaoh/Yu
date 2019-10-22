
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowInstanceNode : BaseEntity<Guid>
    {
        // 工作流实例ID
        public Guid InstanceId { get; set; }

        // 流程节点ID
        public Guid NodeId { get; set; }

        // 处理人ID
        public string HandlePeoples { get; set; }

        // 处理人ID
        public string HandlePeopleNames { get; set; }

        // 处理结果
        // 0.未处理
        // 1.待处理
        // 2.拒绝
        // 3.同意
        public int handleStatus { get; set; }

        // 处理说明
        public string Explain { get; set; }

        // 处理时间
        public DateTime HandleDateTime { get; set; }
    }
}

